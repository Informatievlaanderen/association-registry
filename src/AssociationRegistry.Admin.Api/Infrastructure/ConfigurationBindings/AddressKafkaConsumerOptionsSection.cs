namespace AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;

public class AddressKafkaConsumerOptionsSection
{
    public const string SectionName = "AddressKafkaConsumer";

    public bool Enabled { get; set; }
    public string BootstrapServer { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string GroupId { get; set; }
    public string TopicName { get; set; }
    public int Offset { get; set; }
}
