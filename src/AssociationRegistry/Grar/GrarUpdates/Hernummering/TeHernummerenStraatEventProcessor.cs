namespace AssociationRegistry.Grar.GrarUpdates.Hernummering;

using Framework;

public class TeHernummerenStraatEventProcessor: ITeHernummerenStraatEventProcessor
{
    private readonly TeHeradresserenLocatiesMapper _teHeradresserenLocatiesMapper;
    private readonly ISqsClientWrapper _sqsClientWrapper;

    public TeHernummerenStraatEventProcessor(
        TeHeradresserenLocatiesMapper teHeradresserenLocatiesMapper,
        ISqsClientWrapper sqsClientWrapper)
    {
        _teHeradresserenLocatiesMapper = teHeradresserenLocatiesMapper;
        _sqsClientWrapper = sqsClientWrapper;
    }

    public async Task Process(TeHernummerenStraat teHernummerenStraat, string idempotenceKey)
    {
        var heradresseerLocatiesMessages =
            await _teHeradresserenLocatiesMapper.ForAddress(teHernummerenStraat,
                                                            idempotenceKey);

        foreach (var heradresseerLocatiesMessage in heradresseerLocatiesMessages)
        {
            await _sqsClientWrapper.QueueReaddressMessage(heradresseerLocatiesMessage);
        }
    }
}

public interface ITeHernummerenStraatEventProcessor
{
    Task Process(TeHernummerenStraat teHernummerenStraat, string idempotenceKey);
}
