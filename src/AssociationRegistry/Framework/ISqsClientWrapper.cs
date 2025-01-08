namespace AssociationRegistry.Framework;

using Grar.GrarConsumer.Messaging.HeradresseerLocaties;

public interface ISqsClientWrapper
{
    Task QueueReaddressMessage(HeradresseerLocatiesMessage message);
    Task QueueMessage<TMessage>(TMessage message);
    Task QueueKboNummerToSynchronise(string kboNummer);
}
