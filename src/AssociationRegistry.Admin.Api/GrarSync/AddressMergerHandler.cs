namespace AssociationRegistry.Admin.Api.GrarSync;

using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Infrastructure.AWS;

public interface IAdresMergerHandler
{
    Task Handle(AddressWasRetiredBecauseOfMunicipalityMerger addressWasRetiredMessage);
    Task Handle(AddressWasRejectedBecauseOfMunicipalityMerger addressWasRetiredMessage);
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

    public async Task Handle(AddressWasRetiredBecauseOfMunicipalityMerger addressWasRetiredMessage)
        => await SendTeHeradresserenMessage(addressWasRetiredMessage.AddressPersistentLocalId);

    public async Task Handle(AddressWasRejectedBecauseOfMunicipalityMerger addressWasRejectedMessage)
        => await SendTeHeradresserenMessage(addressWasRejectedMessage.AddressPersistentLocalId);

    private async Task SendTeHeradresserenMessage(int adresId)
    {
        var messages = await _teHeradresserenLocatiesFinder.Find(adresId);

        foreach (var message in messages)
        {
            await _sqsClientWrapper.QueueReaddressMessage(message);
        }
    }
}
