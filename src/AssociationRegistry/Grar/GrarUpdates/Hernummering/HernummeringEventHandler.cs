namespace AssociationRegistry.Grar.GrarUpdates.Hernummering;

using Framework;
using Groupers;

public class HernummeringEventHandler: IHernummeringEventHandler
{
    private readonly TeHeradresserenLocatiesMapper _teHeradresserenLocatiesMapper;
    private readonly ISqsClientWrapper _sqsClientWrapper;

    public HernummeringEventHandler(
        TeHeradresserenLocatiesMapper teHeradresserenLocatiesMapper,
        ISqsClientWrapper sqsClientWrapper)
    {
        _teHeradresserenLocatiesMapper = teHeradresserenLocatiesMapper;
        _sqsClientWrapper = sqsClientWrapper;
    }

    public async Task Handle(TeHernummerenStraat teHernummerenStraat, string idempotenceKey)
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

public interface IHernummeringEventHandler
{
}
