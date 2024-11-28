namespace AssociationRegistry.Admin.Api.GrarSync;

using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Infrastructure.AWS;

public interface IAdresMergerHandler
{
    Task Handle(int adresId);
}

public class AdresMergerHandler : IAdresMergerHandler
{
    private readonly ISqsClientWrapper _sqsClientWrapper;
    private readonly ITeHeradresserenLocatiesFinder _teHeradresserenLocatiesFinder;

    public AdresMergerHandler(ISqsClientWrapper sqsClientWrapper, ITeHeradresserenLocatiesFinder teHeradresserenLocatiesFinder)
    {
        _sqsClientWrapper = sqsClientWrapper;
        _teHeradresserenLocatiesFinder = teHeradresserenLocatiesFinder;
    }

    public async Task Handle(int adresId)
    {
        var messages = await _teHeradresserenLocatiesFinder.Find(adresId);

        foreach (var message in messages)
        {
            await _sqsClientWrapper.QueueReaddressMessage(message);
        }
    }
}
