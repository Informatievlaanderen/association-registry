namespace AssociationRegistry.Test.Admin.Api.Framework;

using Marten;
using Marten.Events;
using Npgsql;
using Weasel.Core;

public static class TestDocumentStoreFactory
{
    private const string DatabaseName = "integrationtests";

    public static DocumentStore Create()
    {
        EnsureDbExists(GetRootConnectionString(), DatabaseName);
        var documentStore = DocumentStore.For(options =>
        {
            options.Connection(GetConnectionString(DatabaseName));
            options.Events.StreamIdentity = StreamIdentity.AsString;

            options.AutoCreateSchemaObjects = AutoCreate.All;

            options.CreateDatabasesForTenants(c =>
            {
                // Specify a db to which to connect in case database needs to be created.
                // If not specified, defaults to 'postgres' on the connection for a tenant.
                c.MaintenanceDatabase(GetRootConnectionString());

                c.ForTenant()
                 .CheckAgainstPgDatabase()
                 .WithOwner("root")
                 .WithEncoding("UTF-8")
                 .ConnectionLimit(-1);
            });
        });

        return documentStore;
    }

    private static void EnsureDbExists(string rootConnectionString, string databaseName)
    {
        using var connection = new NpgsqlConnection(rootConnectionString);

        using var cmd = connection.CreateCommand();

        try
        {
            connection.Open();
            cmd.CommandText += $"CREATE DATABASE {databaseName} WITH OWNER = root;";
            cmd.ExecuteNonQuery();
        }
        catch (PostgresException ex)
        {
            if (ex.MessageText != $"database \"{databaseName}\" already exists")
                throw;
        }
        finally
        {
            connection.Close();
            connection.Dispose();
        }
    }

    private static string GetConnectionString(string databaseName)
        => $"host=127.0.0.1;" +
           $"database={databaseName};" +
           $"password=root;" +
           $"username=root";

    private static string GetRootConnectionString()
            => $"host=127.0.0.1;" +
               $"database=verenigingsregister;" +
               $"password=root;" +
               $"username=root";
}
