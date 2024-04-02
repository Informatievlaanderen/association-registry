namespace AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;

public class AddressMatchOptionsSection
{
    public const string SectionName = "AddressMatchOptions";
    public string AddressMatchSqsQueueName { get; set; }
}
