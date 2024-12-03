namespace AssociationRegistry.Framework;

using Acties.GrarConsumer.HeradresseerLocaties;

public interface ISqsClientWrapper
{
    Task QueueReaddressMessage(HeradresseerLocatiesMessage message);
    Task QueueMessage<TMessage>(TMessage message);
    Task QueueKboNummerToSynchronise(string kboNummer);
}
