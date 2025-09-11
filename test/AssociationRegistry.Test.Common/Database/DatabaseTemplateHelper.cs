namespace AssociationRegistry.Test.Common.Database;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

public static class DatabaseTemplateHelper
{
    private const string RootDatabase = "postgres";
    private const string GoldenMasterTemplate = "golden_master_template";
    
    public static void CreateDatabaseFromTemplate(
        IConfiguration configuration, 
        string targetDatabaseName,
        ILogger? logger = null)
    {
        var connectionString = GetConnectionString(configuration, RootDatabase);
        
        using var connection = new NpgsqlConnection(connectionString);
        using var cmd = connection.CreateCommand();
        
        try
        {
            connection.Open();
            
            logger?.LogInformation("Creating database {DatabaseName} from template {Template}", 
                targetDatabaseName, GoldenMasterTemplate);
            
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
    
    private static string GetConnectionString(IConfiguration configuration, string database)
    {
        return $"host={configuration["PostgreSQLOptions:host"]};" +
               $"database={database};" +
               $"password={configuration["PostgreSQLOptions:password"]};" +
               $"username={configuration["PostgreSQLOptions:username"]}";
    }
}