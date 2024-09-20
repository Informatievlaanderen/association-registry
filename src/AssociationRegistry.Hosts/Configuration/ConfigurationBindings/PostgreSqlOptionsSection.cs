namespace AssociationRegistry.Hosts.Configuration.ConfigurationBindings;

public class PostgreSqlOptionsSection
{
    public const string SectionName = "PostgreSQLOptions";
    public string? Host { get; set; }
    public string? Database { get; set; }
    public string? Password { get; set; }
    public string? Username { get; set; }
    public string? Schema { get; set; }

    public bool IsComplete
        => !string.IsNullOrWhiteSpace(Host) &&
           !string.IsNullOrWhiteSpace(Database) &&
           !string.IsNullOrWhiteSpace(Password) &&
           !string.IsNullOrWhiteSpace(Username);

    public string GetConnectionString()
        => $"host={Host};" +
           $"database={Database};" +
           $"password={Password};" +
           $"username={Username};" +
           $"options='-c lock_timeout=30000';";

}
