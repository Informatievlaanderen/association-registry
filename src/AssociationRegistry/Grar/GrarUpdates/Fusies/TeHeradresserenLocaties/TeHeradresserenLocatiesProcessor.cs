namespace AssociationRegistry.Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;

using Framework;
using GrarConsumer.Messaging;
using LocatieFinder;

public class TeHeradresserenLocatiesProcessor : ITeHeradresserenLocatiesProcessor
{
    private readonly ISqsClientWrapper _sqsClientWrapper;
    private readonly ILocatieFinder _locatieFinder;

    public TeHeradresserenLocatiesProcessor(ISqsClientWrapper sqsClientWrapper, ILocatieFinder locatieFinder)
    {
        _sqsClientWrapper = sqsClientWrapper;
        _locatieFinder = locatieFinder;
    }

    public async Task Process(int sourceAdresId, int destinationAdresId, string idempotencyKey)
    {
        var locatiesMetVCodes = await _locatieFinder.FindLocaties(sourceAdresId);

        var messages = locatiesMetVCodes.Map(destinationAdresId, idempotencyKey);

        foreach (var message in messages)
        {
            await _sqsClientWrapper.QueueMessage(
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
