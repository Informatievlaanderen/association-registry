namespace AssociationRegistry.Grar.AdresMatch;

using Clients;
using DecentraalBeheer.Vereniging;
using Events;
using Events.Factories;
using GemeentenaamVerrijking;
using Vereniging;

public class AdresMatchService
{
    public static async Task<IEvent> GetAdresMatchEvent(
        int locatieId,
        Locatie locatie,
        IGrarClient grarClient,
        CancellationToken cancellationToken,
        VCode vCode)
    {
        if (locatie is null)
        {
            return new AdresKonNietOvergenomenWordenUitAdressenregister(vCode,
                                                                        locatieId,
                                                                        string.Empty,
                                                                        AdresKonNietOvergenomenWordenUitAdressenregister
                                                                           .RedenLocatieWerdVerwijderd);
        }

        var adresMatches = await grarClient.GetAddressMatches(
            locatie.Adres.Straatnaam,
            locatie.Adres.Huisnummer,
            locatie.Adres.Busnummer,
            locatie.Adres.Postcode,
            locatie.Adres.Gemeente.Naam,
            cancellationToken);


        if (adresMatches.HasNoResponse)
            return EventFactory.AdresWerdNietGevondenInAdressenregister(vCode, locatie);

        var adresMatchesSingular100ScoreResponse = adresMatches.Singular100ScoreResponse;

        if (adresMatchesSingular100ScoreResponse is null)
            return new AdresNietUniekInAdressenregister(vCode, locatieId,
                                                        adresMatches.Select(EventFactory.NietUniekeAdresMatchUitAdressenregister)
                                                                    .ToArray());

        var postalInformation = await grarClient.GetPostalInformationDetail(locatie.Adres.Postcode);
        var verrijkteGemeentenaam = GemeentenaamDecorator.VerrijkGemeentenaam(
            locatie.Adres.Gemeente,
            postalInformation,
            adresMatchesSingular100ScoreResponse.Gemeente);

        var registratieData =
            EventFactory.AdresUitAdressenregister(
                adresMatchesSingular100ScoreResponse, verrijkteGemeentenaam);

        return new AdresWerdOvergenomenUitAdressenregister(vCode, locatieId,
                                                           adresMatchesSingular100ScoreResponse.AdresId!,
                                                           registratieData);
    }
}
