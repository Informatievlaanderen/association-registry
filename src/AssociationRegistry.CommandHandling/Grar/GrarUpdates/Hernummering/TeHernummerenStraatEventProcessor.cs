namespace AssociationRegistry.CommandHandling.Grar.GrarUpdates.Hernummering;

using GrarConsumer.Messaging;
using Wolverine;

public class TeHernummerenStraatEventProcessor: ITeHernummerenStraatEventProcessor
{
    private readonly TeHeradresserenLocatiesMapper _teHeradresserenLocatiesMapper;
    private readonly IMessageBus _messageBus;

    public TeHernummerenStraatEventProcessor(
        TeHeradresserenLocatiesMapper teHeradresserenLocatiesMapper,
        IMessageBus messageBus)
    {
        _teHeradresserenLocatiesMapper = teHeradresserenLocatiesMapper;
        _messageBus = messageBus;
    }

    public async Task Process(TeHernummerenStraat teHernummerenStraat, string idempotenceKey)
    {
        var heradresseerLocatiesMessages =
            await _teHeradresserenLocatiesMapper.ForAddress(teHernummerenStraat,
                                                            idempotenceKey);

        foreach (var heradresseerLocatiesMessage in heradresseerLocatiesMessages)
        {
            await _messageBus.SendAsync(
                new OverkoepelendeGrarConsumerMessage
                {
                    HeradresseerLocatiesMessage = heradresseerLocatiesMessage,
                });
        }
    }
}

public interface ITeHernummerenStraatEventProcessor
{
    Task Process(TeHernummerenStraat teHernummerenStraat, string idempotenceKey);
}
