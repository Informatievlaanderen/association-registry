namespace AssociationRegistry.Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;

using Framework;
using LocatieFinder;

public class TeOntkoppelenLocatiesProcessor : ITeOntkoppelenLocatiesProcessor
{
    private readonly ISqsClientWrapper _sqsClientWrapper;
    private readonly ILocatieFinder _locatieFinder;

    public TeOntkoppelenLocatiesProcessor(ISqsClientWrapper sqsClientWrapper, ILocatieFinder locatieFinder)
    {
        _sqsClientWrapper = sqsClientWrapper;
        _locatieFinder = locatieFinder;
    }

    public async Task Process(int sourceAdresId)
    {
        var locatiesMetVCodes = await _locatieFinder.FindLocaties(sourceAdresId);

        var messages = locatiesMetVCodes.Map();

        foreach (var message in messages)
        {
            await _sqsClientWrapper.QueueMessage(message);
        }
    }
}

public interface ITeOntkoppelenLocatiesProcessor
{
    Task Process(int sourceAdresId);
}
