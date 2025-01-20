namespace AssociationRegistry.KboMutations.SyncLambda.Logging;

public class PostgreSqlOptionsSection
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