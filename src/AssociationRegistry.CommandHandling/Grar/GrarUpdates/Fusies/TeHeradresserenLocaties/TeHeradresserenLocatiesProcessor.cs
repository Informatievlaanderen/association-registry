namespace AssociationRegistry.CommandHandling.Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;

using GrarConsumer.Messaging;
using LocatieFinder;
using Wolverine;

public class TeHeradresserenLocatiesProcessor : ITeHeradresserenLocatiesProcessor
{
    private readonly IMessageBus _messageBus;
    private readonly ILocatieFinder _locatieFinder;

    public TeHeradresserenLocatiesProcessor(IMessageBus messageBus, ILocatieFinder locatieFinder)
    {
        _messageBus = messageBus;
        _locatieFinder = locatieFinder;
    }

    public async Task Process(int sourceAdresId, int destinationAdresId, string idempotencyKey)
    {
        var locatiesMetVCodes = await _locatieFinder.FindLocaties(sourceAdresId);

        var messages = locatiesMetVCodes.Map(destinationAdresId, idempotencyKey);

        foreach (var message in messages)
        {
            await _messageBus.SendAsync(
                new OverkoepelendeGrarConsumerMessage
                {
                    HeradresseerLocatiesMessage = message,
                });
        }
    }
}

public interface ITeHeradresserenLocatiesProcessor
{
    Task Process(int sourceAdresId, int destinationAdresId, string idempotencyKey);
}
