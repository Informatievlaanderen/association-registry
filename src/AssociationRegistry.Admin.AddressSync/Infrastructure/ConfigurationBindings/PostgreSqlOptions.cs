namespace AssociationRegistry.Admin.AddressSync.Infrastructure.ConfigurationBindings;

public class PostgreSqlOptions
{
    public const string SectionName = "PostgreSQLOptions";
    public string? Host { get; set; }
    public string? Database { get; set; }
    public string? Password { get; set; }
    public string? Username { get; set; }

    public bool IsComplete
        => !string.IsNullOrWhiteSpace(Host) &&
           !string.IsNullOrWhiteSpace(Database) &&
           !string.IsNullOrWhiteSpace(Password) &&
           !string.IsNullOrWhiteSpace(Username);
}
