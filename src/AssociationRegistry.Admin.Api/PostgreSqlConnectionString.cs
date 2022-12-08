namespace AssociationRegistry.Admin.Api;

using ConfigurationBindings;

public class PostgreSqlConnectionString
{
    public PostgreSqlConnectionString(PostgreSqlOptionsSection postgreSqlOptions)
    {
        UserName = postgreSqlOptions.Username;
        Value = $"host={postgreSqlOptions.Host};" +
                $"database={postgreSqlOptions.Database};" +
                $"password={postgreSqlOptions.Password};" +
                $"username={postgreSqlOptions.Username}";
    }

    public string Value { get; }

    public string UserName { get; set; }

    public static implicit operator string(PostgreSqlConnectionString value)
    {
        return value.Value;
    }
}
