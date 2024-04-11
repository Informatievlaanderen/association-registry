namespace AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;

public class AddressMatchOptionsSection
{
    public const string SectionName = "AddressMatchOptions";
    public string AddressMatchSqsQueueName { get; set; }
    public bool UseLocalStack { get; set; }
    public string SqsTransportServiceUrl { get; set; }
}
