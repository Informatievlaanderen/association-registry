namespace AssociationRegistry.Framework;

using Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;

public interface ISqsClientWrapper
{
    Task QueueReaddressMessage(TeHeradresserenLocatiesMessage message);
    Task QueueMessage<TMessage>(TMessage message);
    Task QueueKboNummerToSynchronise(string kboNummer);
}
