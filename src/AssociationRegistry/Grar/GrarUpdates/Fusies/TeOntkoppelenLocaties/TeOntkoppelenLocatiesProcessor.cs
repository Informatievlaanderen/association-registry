namespace AssociationRegistry.Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;

using Framework;
using GrarConsumer.Messaging;
using LocatieFinder;
using Wolverine;

public class TeOntkoppelenLocatiesProcessor : ITeOntkoppelenLocatiesProcessor
{
    private readonly IMessageBus _messageBus;
    private readonly ILocatieFinder _locatieFinder;

    public TeOntkoppelenLocatiesProcessor(IMessageBus messageBus, ILocatieFinder locatieFinder)
    {
        _messageBus = messageBus;
        _locatieFinder = locatieFinder;
    }

    public async Task Process(int sourceAdresId)
    {
        var locatiesMetVCodes = await _locatieFinder.FindLocaties(sourceAdresId);

        var messages = locatiesMetVCodes.Map();

        foreach (var message in messages)
        {
            await _messageBus.SendAsync(
                new OverkoepelendeGrarConsumerMessage
                {
                    OntkoppelLocatiesMessage = message,
                });
        }
    }
}

public interface ITeOntkoppelenLocatiesProcessor
{
    Task Process(int sourceAdresId);
}
