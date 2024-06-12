namespace AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;

public class GrarSyncOptionsSection
{
    public const string SectionName = "GrarSyncOptions";
    public string GrarSyncSqsQueueName { get; set; }
    public string GrarSyncSqsDeadLetterQueueName { get; set; }
    public bool Enabled { get; set; }
}
