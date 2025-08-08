namespace AssociationRegistry.Grar.AdresMatch.Infrastructure;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Events;
using AssociationRegistry.Events.Factories;
using AssociationRegistry.GemeentenaamVerrijking;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Vereniging;
using Domain;

public class GemeenteVerrijkingService : IAdresVerrijkingService
{
    private readonly IGrarClient _grarClient;

    public GemeenteVerrijkingService(IGrarClient grarClient)
    {
        _grarClient = grarClient;
    }

    public async Task<Registratiedata.AdresUitAdressenregister> VerrijkAdres(
        AddressMatchResponse matchResponse,
        Adres origineleAdres,
        CancellationToken cancellationToken)
    {
        var postalInformation = await _grarClient.GetPostalInformationDetail(origineleAdres.Postcode);
        
        var verrijkteGemeentenaam = GemeentenaamDecorator.VerrijkGemeentenaam(
            origineleAdres.Gemeente,
            postalInformation,
            matchResponse.Gemeente);

        return EventFactory.AdresUitAdressenregister(matchResponse, verrijkteGemeentenaam);
    }
}