namespace AssociationRegistry.Test.E2E.Framework.Mappers;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Admin.Schema.Historiek.EventData;
using AlbaHost;
using Events;
using EventStore;

public static class HistoriekGebeurtenisMapper
{
    public static HistoriekGebeurtenisResponse FeitelijkeVerenigingWerdGeregistreerd(RegistreerFeitelijkeVerenigingRequest request, string vCode)
    {
        return new ()
        {
            Beschrijving = $"Feitelijke vereniging werd geregistreerd met naam '{request.Naam}'.",
            Gebeurtenis = nameof(Events.FeitelijkeVerenigingWerdGeregistreerd),
            Data = new FeitelijkeVerenigingWerdGeregistreerdData(
                VCode: vCode,
                Naam: request.Naam,
                KorteNaam: request.KorteNaam!,
                KorteBeschrijving: request.KorteBeschrijving!,
                Startdatum: request.Startdatum,
                Doelgroep: new Registratiedata.Doelgroep(
                    request.Doelgroep!.Minimumleeftijd!.Value,
                    request.Doelgroep.Maximumleeftijd!.Value),
                IsUitgeschrevenUitPubliekeDatastroom: request
                                                             .IsUitgeschrevenUitPubliekeDatastroom,
                Contactgegevens: request.Contactgegevens.Select(
                                             (x, i) => new Registratiedata.Contactgegeven(
                                                 i + 1,
                                                 x.Contactgegeventype,
                                                 x.Waarde,
                                                 x.Beschrijving!,
                                                 x.IsPrimair))
                                        .ToArray(),
                Locaties: request.Locaties.Select(
                                      (x, i) => new Registratiedata.Locatie(
                                          i + 1,
                                          x.Locatietype,
                                          x.IsPrimair,
                                          x.Naam!,
                                          x.Adres == null
                                              ? null
                                              : new Registratiedata.Adres(
                                                  x.Adres.Straatnaam,
                                                  x.Adres.Huisnummer,
                                                  x.Adres.Busnummer!,
                                                  x.Adres.Postcode,
                                                  x.Adres.Gemeente,
                                                  x.Adres.Land
                                              ),
                                          x.AdresId == null
                                              ? null
                                              : new Registratiedata.AdresId(x.AdresId.Broncode, x.AdresId.Bronwaarde)))
                                 .ToArray(),
                Vertegenwoordigers: null,
                HoofdactiviteitenVerenigingsloket: null),
            Initiator = AuthenticationSetup.Initiator,
        };
    }

    public static HistoriekGebeurtenisResponse AdresWerdOvergenomen(string vCode)
        => new()
        {
            Beschrijving = "Adres werd overgenomen uit het adressenregister.",
            Gebeurtenis = nameof(AdresWerdOvergenomenUitAdressenregister),
            Data = new AdresWerdOvergenomenUitAdressenregister(
                vCode,
                LocatieId: 1,
                Adres: new Registratiedata.AdresUitAdressenregister(
                    Straatnaam: "Leopold II-laan",
                    Huisnummer: "99",
                    Busnummer: "",
                    Postcode: "9200",
                    Gemeente: "Dendermonde"),
                AdresId: new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/3213019")),
            Initiator = EventStore.DigitaalVlaanderenOvoNumber,
        };

    public static HistoriekGebeurtenisResponse AdresNietUniekInAR(string vCode)
    {
        return new()
        {
            Beschrijving = "Adres niet uniek in het adressenregister.",
            Gebeurtenis = nameof(AdresNietUniekInAdressenregister),
            Data = new AdresNietUniekInAdressenregister(
                vCode,
                LocatieId: 2,
                new NietUniekeAdresMatchUitAdressenregister[]
                {
                    new()
                    {
                        AdresId =
                            new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/3213019"),
                        Adresvoorstelling = "Leopold II-laan 99, 9200 Dendermonde",
                        Score = 95.4942299939528,
                    },
                    new()
                    {
                        AdresId = new Registratiedata.AdresId(Broncode: "AR",
                                                              Bronwaarde: "https://data.vlaanderen.be/id/adres/20055852"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 1, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId =
                            new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/5459542"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 2, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId =
                            new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/5556720"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 3, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId =
                            new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/5435137"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 6, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId =
                            new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/5451858"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 7, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId = new Registratiedata.AdresId(Broncode: "AR",
                                                              Bronwaarde: "https://data.vlaanderen.be/id/adres/19038299"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 8, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId = new Registratiedata.AdresId(Broncode: "AR",
                                                              Bronwaarde: "https://data.vlaanderen.be/id/adres/20172048"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 9, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId =
                            new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/5656904"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 10, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                    new()
                    {
                        AdresId =
                            new Registratiedata.AdresId(Broncode: "AR", Bronwaarde: "https://data.vlaanderen.be/id/adres/5512213"),
                        Adresvoorstelling = "Leopold II-laan 99 bus 11, 9200 Dendermonde",
                        Score = 86.987355612507,
                    },
                }),
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:04Z",
        };
    }

    public static HistoriekGebeurtenisResponse AdresKonNietOvergenomenWorden(string vCode)
        => new()
        {
            Beschrijving = "Adres kon niet gevonden worden in het adressenregister.",
            Gebeurtenis = nameof(AdresWerdNietGevondenInAdressenregister),
            Data = new
            {
                vCode = vCode,
                locatieId = 3,
                straatnaam = "dorpelstraat",
                huisnummer = "169",
                busnummer = "2",
                postcode = "4567",
                gemeente = "Nothingham",
            },
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        };

}
