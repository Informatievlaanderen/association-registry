namespace AssociationRegistry.Test.Common.Database;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

public static class DatabaseTemplateHelper
{
    private const string RootDatabase = "postgres";
    private const string GoldenMasterTemplate = "golden_master_template";
    private const string GoldenMasterSchema = "golden_master_schema";
    private static readonly object _templateLock = new object();
    
    public static void CreateDatabaseFromTemplate(
        IConfiguration configuration, 
        string targetDatabaseName,
        ILogger? logger = null)
    {
        lock (_templateLock)
        {
            var connectionString = GetConnectionString(configuration, RootDatabase);
            
            using var connection = new NpgsqlConnection(connectionString);
            using var cmd = connection.CreateCommand();
            
            try
            {
                connection.Open();
                
                logger?.LogInformation("Creating database {DatabaseName} from template {Template}", 
                    targetDatabaseName, GoldenMasterTemplate);
                
                // Wait for template database to be available
                WaitForTemplateAvailable(cmd, logger);
                
                // Drop the target database if it exists
                cmd.CommandText = $"DROP DATABASE IF EXISTS \"{targetDatabaseName}\" WITH (FORCE);";
                cmd.ExecuteNonQuery();
                
                // Create database from golden master template - this is much faster than running migrations
                cmd.CommandText = $"CREATE DATABASE \"{targetDatabaseName}\" WITH TEMPLATE \"{GoldenMasterTemplate}\";";
                cmd.ExecuteNonQuery();
                
                logger?.LogInformation("Successfully created database {DatabaseName} from template", targetDatabaseName);
            }
            catch (PostgresException ex)
            {
                logger?.LogError(ex, "Failed to create database {DatabaseName} from template", targetDatabaseName);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
    }
    
    public static void CreateSchemaFromTemplate(
        IConfiguration configuration,
        string targetSchemaName,
        string targetDatabase = GoldenMasterTemplate,
        ILogger? logger = null)
    {
        lock (_templateLock)
        {
            var connectionString = GetConnectionString(configuration, targetDatabase);
            
            using var connection = new NpgsqlConnection(connectionString);
            using var cmd = connection.CreateCommand();
            
            try
            {
                connection.Open();
                
                logger?.LogInformation("Creating schema {SchemaName} from template schema {TemplateSchema} in database {Database}", 
                    targetSchemaName, GoldenMasterSchema, targetDatabase);
                
                // Drop the target schema if it exists
                cmd.CommandText = $"DROP SCHEMA IF EXISTS \"{targetSchemaName}\" CASCADE;";
                cmd.ExecuteNonQuery();
                
                // Create the new schema
                cmd.CommandText = $"CREATE SCHEMA \"{targetSchemaName}\";";
                cmd.ExecuteNonQuery();
                
                // Copy all tables, indexes, sequences, and data from the golden master schema
                cmd.CommandText = $"""
                    DO $$
                    DECLARE
                        rec RECORD;
                        sql_stmt TEXT;
                    BEGIN
                        -- Copy tables structure and data
                        FOR rec IN 
                            SELECT schemaname, tablename 
                            FROM pg_tables 
                            WHERE schemaname = '{GoldenMasterSchema}'
                        LOOP
                            sql_stmt := format('CREATE TABLE %I.%I (LIKE %I.%I INCLUDING ALL)',
                                '{targetSchemaName}', rec.tablename, '{GoldenMasterSchema}', rec.tablename);
                            EXECUTE sql_stmt;
                            
                            sql_stmt := format('INSERT INTO %I.%I SELECT * FROM %I.%I',
                                '{targetSchemaName}', rec.tablename, '{GoldenMasterSchema}', rec.tablename);
                            EXECUTE sql_stmt;
                        END LOOP;
                        
                        -- Copy sequences and set their values
                        FOR rec IN 
                            SELECT schemaname, sequencename 
                            FROM pg_sequences 
                            WHERE schemaname = '{GoldenMasterSchema}'
                        LOOP
                            sql_stmt := format('CREATE SEQUENCE %I.%I',
                                '{targetSchemaName}', rec.sequencename);
                            EXECUTE sql_stmt;
                            
                            sql_stmt := format('SELECT setval(%L, (SELECT last_value FROM %I.%I))',
                                '{targetSchemaName}.' || rec.sequencename, '{GoldenMasterSchema}', rec.sequencename);
                            EXECUTE sql_stmt;
                        END LOOP;
                        
                        -- Copy functions and procedures
                        FOR rec IN 
                            SELECT p.proname, p.oid
                            FROM pg_proc p
                            JOIN pg_namespace n ON p.pronamespace = n.oid
                            WHERE n.nspname = '{GoldenMasterSchema}'
                        LOOP
                            sql_stmt := pg_get_functiondef(rec.oid);
                            sql_stmt := replace(sql_stmt, '{GoldenMasterSchema}.', '{targetSchemaName}.');
                            EXECUTE sql_stmt;
                        END LOOP;
                    END
                    $$;
                    """;
                cmd.ExecuteNonQuery();
                
                logger?.LogInformation("Successfully created schema {SchemaName} from template", targetSchemaName);
            }
            catch (PostgresException ex)
            {
                logger?.LogError(ex, "Failed to create schema {SchemaName} from template", targetSchemaName);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
    }

    public static bool IsGoldenMasterTemplateAvailable(IConfiguration configuration, ILogger? logger = null)
    {
        var connectionString = GetConnectionString(configuration, RootDatabase);
        
        using var connection = new NpgsqlConnection(connectionString);
        using var cmd = connection.CreateCommand();
        
        try
        {
            connection.Open();
            cmd.CommandText = $"SELECT 1 FROM pg_database WHERE datname = '{GoldenMasterTemplate}';";
            var result = cmd.ExecuteScalar();
            return result != null;
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Failed to check if golden master template exists");
            return false;
        }
        finally
        {
            connection.Close();
        }
    }
    
    private static void WaitForTemplateAvailable(NpgsqlCommand cmd, ILogger? logger)
    {
        var maxAttempts = 30; // 30 seconds max wait
        var delayMs = 1000; // 1 second between attempts
        
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                // Check if there are any active connections to the template database
                cmd.CommandText = $"""
                    SELECT COUNT(*) 
                    FROM pg_stat_activity 
                    WHERE datname = '{GoldenMasterTemplate}' 
                    AND state = 'active'
                    AND pid <> pg_backend_pid()
                    """;
                
                var activeConnections = Convert.ToInt32(cmd.ExecuteScalar());
                
                if (activeConnections == 0)
                {
                    logger?.LogInformation("Template database is available (attempt {Attempt})", attempt);
                    return; // Template is available
                }
                
                logger?.LogInformation("Template database has {ActiveConnections} active connections, waiting... (attempt {Attempt}/{MaxAttempts})", 
                    activeConnections, attempt, maxAttempts);
                
                if (attempt < maxAttempts)
                {
                    Thread.Sleep(delayMs);
                }
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Error checking template database availability (attempt {Attempt}/{MaxAttempts})", attempt, maxAttempts);
                
                if (attempt < maxAttempts)
                {
                    Thread.Sleep(delayMs);
                }
            }
        }
        
        logger?.LogWarning("Template database may still be in use after {MaxAttempts} attempts, proceeding anyway", maxAttempts);
    }
    
    private static string GetConnectionString(IConfiguration configuration, string database)
    {
        return $"host={configuration["PostgreSQLOptions:host"]};" +
               $"database={database};" +
               $"password={configuration["PostgreSQLOptions:password"]};" +
               $"username={configuration["PostgreSQLOptions:username"]}";
    }
}