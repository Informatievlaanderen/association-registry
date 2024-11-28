namespace AssociationRegistry.Admin.Api.GrarSync;

using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Infrastructure.AWS;

public class AdresMergerHandler
{
    private readonly ISqsClientWrapper _sqsClientWrapper;
    private readonly ITeHeradresserenLocatiesFinder _teHeradresserenLocatiesFinder;

    public AdresMergerHandler(ISqsClientWrapper sqsClientWrapper, ITeHeradresserenLocatiesFinder teHeradresserenLocatiesFinder)
    {
        _sqsClientWrapper = sqsClientWrapper;
        _teHeradresserenLocatiesFinder = teHeradresserenLocatiesFinder;
    }

    public async Task Handle(AddressWasRetiredBecauseOfMunicipalityMerger addressWasRetiredMessage)
    {
        var messages = await _teHeradresserenLocatiesFinder.Find(addressWasRetiredMessage.AddressPersistentLocalId);

        foreach (var message in messages)
        {
            await _sqsClientWrapper.QueueReaddressMessage(message);
        }
    }
}
