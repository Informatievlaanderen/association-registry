namespace AssociationRegistry.Framework;

using Acties.HeradresseerLocaties;
using Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;

public interface ISqsClientWrapper
{
    Task QueueReaddressMessage(HeradresseerLocatiesMessage message);
    Task QueueMessage<TMessage>(TMessage message);
    Task QueueKboNummerToSynchronise(string kboNummer);
}
