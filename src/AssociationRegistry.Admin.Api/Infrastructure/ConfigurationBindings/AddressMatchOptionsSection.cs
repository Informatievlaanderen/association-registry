namespace AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;

public class AddressMatchOptionsSection
{
    public const string SectionName = "AddressMatchOptions";
    public string AddressMatchSqsQueueName { get; set; }
    public string AddressMatchSqsDeadLetterQueueName { get; set; }
    public string SqsTransportServiceUrl { get; set; }
    public bool OptimizeArtifactWorkflow { get; set; }
    public bool AutoProvision { get; set; }
    public bool UseLocalStack { get; set; }
}
