namespace AssociationRegistry.Admin.Api.GrarConsumer.Handlers;

using Finders;
using Grar.GrarUpdates;
using Grar.GrarUpdates.TeHeradresserenLocaties;
using Grar.Models;
using Infrastructure.AWS;

public class TeHeradresserenLocatiesHandler : ITeHeradresserenLocatiesHandler
{
    private readonly ISqsClientWrapper _sqsClientWrapper;
    private readonly ILocatieFinder _locatieFinder;

    public TeHeradresserenLocatiesHandler(ISqsClientWrapper sqsClientWrapper, ILocatieFinder locatieFinder)
    {
        _sqsClientWrapper = sqsClientWrapper;
        _locatieFinder = locatieFinder;
    }

    public async Task Handle(int sourceAdresId, int destinationAdresId)
    {
        var locatiesMetVCodes = await _locatieFinder.FindLocaties(sourceAdresId);

        var messages = locatiesMetVCodes.Map(destinationAdresId);

        foreach (var message in messages)
        {
            await _sqsClientWrapper.QueueReaddressMessage(message);
        }
    }
}

