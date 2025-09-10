namespace AssociationRegistry.Test.Common.Database;

using Extensions;
using Npgsql;
using System.Reflection;

public static class NpgSqlConnectionExtensions
{
    public static void EnsureSchemasExists(this NpgsqlConnection connection, string databaseName)
    {
        using var ensureSchemasCommand = connection.CreateCommand();

        try
        {
            connection.Open();
            ensureSchemasCommand.CommandText += typeof(NpgSqlConnectionExtensions).Assembly.GetResourceString("AssociationRegistry.Test.Common.Database.schema_only_tst.sql");
            ensureSchemasCommand.ExecuteNonQuery();
        }
        catch (PostgresException ex)
        {
            if (ex.MessageText != $"database \"{databaseName.ToLower()}\" already exists")
                throw;
        }
    }
}
