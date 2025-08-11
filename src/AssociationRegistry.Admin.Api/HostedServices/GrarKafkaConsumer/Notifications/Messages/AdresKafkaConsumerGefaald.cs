namespace AssociationRegistry.Admin.Api.HostedServices.GrarKafkaConsumer.Notifications.Messages;

using Integrations.Slack;

public readonly record struct AdresKafkaConsumerGefaald : INotification
{
    private readonly Exception _exception;

    public AdresKafkaConsumerGefaald(Exception exception)
    {
        _exception = exception;
    }

    public string Value => $"Address Kafka Consumer heeft onverwachte fout! {_exception.Message}";
    public NotifyType Type => NotifyType.Failure;
}
