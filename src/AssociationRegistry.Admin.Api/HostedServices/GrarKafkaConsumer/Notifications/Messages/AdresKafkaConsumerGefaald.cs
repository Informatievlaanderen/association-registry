namespace AssociationRegistry.Admin.Api.GrarConsumer.Notifications.Messages;

using AssociationRegistry.Notifications;

public readonly record struct AdresKafkaConsumerGefaald : IMessage
{
    private readonly Exception _exception;

    public AdresKafkaConsumerGefaald(Exception exception)
    {
        _exception = exception;
    }

    public string Value => $"Address Kafka Consumer heeft onverwachte fout! {_exception.Message}";
    public NotifyType Type => NotifyType.Failure;
}
