namespace AssociationRegistry.Integrations.Grar.AdresMatch;

using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Events;
using AssociationRegistry.Events.Factories;
using AssociationRegistry.GemeentenaamVerrijking;
using AssociationRegistry.Grar.AdresMatch;
using AssociationRegistry.Grar.Exceptions;
using AssociationRegistry.Grar.Models;
using Clients;
using DecentraalBeheer.Vereniging.Adressen.GemeentenaamVerrijking;

public class GrarAddressVerrijkingsService : IAddressVerrijkingsService
{
    private readonly IGrarClient _grarClient;

    public GrarAddressVerrijkingsService(IGrarClient grarClient)
    {
        _grarClient = grarClient;
    }

    public async Task<VerrijktAdresUitGrar> FromAdresAndGrarResponse(
        IAddressResponse matchResponse,
        Adres origineleAdres,
        CancellationToken cancellationToken)
    {
        var postalInformation = await _grarClient.GetPostalInformationDetail(origineleAdres.Postcode);

        var verrijkteGemeentenaam = GemeentenaamDecorator.VerrijkGemeentenaam(
            origineleAdres.Gemeente,
            postalInformation,
            matchResponse.Gemeente);

        return VerrijktAdresUitGrar.FromAdresAndVerrijkteGemeentenaam(
            origineleAdres, verrijkteGemeentenaam, matchResponse);
    }

    public async Task<VerrijktAdresUitGrar> FromActiefAdresId(
        AdresId adresId,
        CancellationToken cancellationToken)
    {
        var adresDetailResponse = await _grarClient.GetAddressById(adresId.ToString(), cancellationToken);

        if (!adresDetailResponse.IsActief)
            throw new AdressenregisterReturnedInactiefAdres();

        var postalInformation = await _grarClient.GetPostalInformationDetail(adresDetailResponse.Postcode);

        var verrijkteGemeentenaam = GemeentenaamDecorator.VerrijkGemeentenaam(
            postalInformation,
            adresDetailResponse.Gemeente);

        return VerrijktAdresUitGrar.FromAdresUitGrarAndVerrijkteGemeentenaam(
            adresDetailResponse, verrijkteGemeentenaam);
    }
}
