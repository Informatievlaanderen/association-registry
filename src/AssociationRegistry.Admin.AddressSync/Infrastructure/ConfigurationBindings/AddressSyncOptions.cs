namespace AssociationRegistry.Admin.AddressSync.Infrastructure.ConfigurationBindings;

public class AddressSyncOptions
{
    public const string SectionName = "AddressSyncOptions";
    public string QueueUrl { get; set; }

    public bool IsComplete
        => !string.IsNullOrWhiteSpace(QueueUrl);
}
