namespace AssociationRegistry.Admin.Api.GrarConsumer.Handlers.Fusies;

using Grar.LocatieFinder;
using Infrastructure.AWS;

public class TeOntkoppelenLocatieHandler : ITeOntkoppelenLocatiesHandler
{
    private readonly ISqsClientWrapper _sqsClientWrapper;
    private readonly ILocatieFinder _locatieFinder;

    public TeOntkoppelenLocatieHandler(ISqsClientWrapper sqsClientWrapper, ILocatieFinder locatieFinder)
    {
        _sqsClientWrapper = sqsClientWrapper;
        _locatieFinder = locatieFinder;
    }

    public async Task Handle(int sourceAdresId)
    {
        var locatiesMetVCodes = await _locatieFinder.FindLocaties(sourceAdresId);

        var messages = locatiesMetVCodes.Map();

        foreach (var message in messages)
        {
            await _sqsClientWrapper.QueueMessage(message);
        }
    }
}
