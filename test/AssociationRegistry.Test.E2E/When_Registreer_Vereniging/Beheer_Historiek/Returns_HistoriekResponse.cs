namespace AssociationRegistry.Test.E2E.When_Registreer_Vereniging.Beheer_Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Schema.Historiek.EventData;
using Alba;
using Events;
using EventStore;
using Framework.AlbaHost;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.Logging;
using Xunit;

[Collection(nameof(RegistreerVerenigingContext))]
public class Adds_WerdGeregistreerd_Gebeurtenis_To_Historiek
{
    private readonly RegistreerVerenigingContext _context;
    private readonly IAlbaHost _adminApiHost;
    private HistoriekResponse _result;

    public Adds_WerdGeregistreerd_Gebeurtenis_To_Historiek(RegistreerVerenigingContext context)
    {
        _context = context;
        _adminApiHost = context.ApiHost;

        _result = _adminApiHost.GetHistoriek(context.ResultingVCode)!;
    }

    [Fact]
    public void With_VCode()
    {
        _result.VCode.ShouldCompare(_context.ResultingVCode);
    }

    [Fact]
    public void With_Context()
    {
        _result.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_All_Gebeurtenissen()
    {
        // Perform the comparison
        _result.Gebeurtenissen.ShouldCompare([
            FeitelijkeVerenigingWerdGeregistreerd(_context),
            AdresWerdOvergenomen(_context),
            AdresNietUniekInAR(_context),
            AdresKonNietOvergenomenWorden(_context),
        ], compareConfig: HistoriekComparisonConfig.Instance);
    }

    private static HistoriekGebeurtenisResponse FeitelijkeVerenigingWerdGeregistreerd(RegistreerVerenigingContext context)
    {
        return new ()
        {
            Beschrijving = $"Feitelijke vereniging werd geregistreerd met naam '{context.Request.Naam}'.",
            Gebeurtenis = nameof(Events.FeitelijkeVerenigingWerdGeregistreerd),
            Data = new FeitelijkeVerenigingWerdGeregistreerdData(
                VCode: context.ResultingVCode,
                Naam: context.Request.Naam,
                KorteNaam: context.Request.KorteNaam!,
                KorteBeschrijving: context.Request.KorteBeschrijving!,
                Startdatum: context.Request.Startdatum,
                Doelgroep: new Registratiedata.Doelgroep(
                    context.Request.Doelgroep!.Minimumleeftijd!.Value,
                    context.Request.Doelgroep.Maximumleeftijd!.Value),
                IsUitgeschrevenUitPubliekeDatastroom: context.Request
                                                             .IsUitgeschrevenUitPubliekeDatastroom,
                Contactgegevens: context.Request.Contactgegevens.Select(
                                             (x, i) => new Registratiedata.Contactgegeven(
                                                 i + 1,
                                                 x.Contactgegeventype,
                                                 x.Waarde,
                                                 x.Beschrijving!,
                                                 x.IsPrimair))
                                        .ToArray(),
                Locaties: context.Request.Locaties.Select(
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
            Initiator = AppFixture.Initiator,
        };
    }

    private static HistoriekGebeurtenisResponse AdresWerdOvergenomen(RegistreerVerenigingContext context)
        => new()
        {
            Beschrijving = "Adres werd overgenomen uit het adressenregister.",
            Gebeurtenis = nameof(AdresWerdOvergenomenUitAdressenregister),
            Data = new AdresWerdOvergenomenUitAdressenregister(
                context.ResultingVCode,
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

    private static HistoriekGebeurtenisResponse AdresNietUniekInAR(RegistreerVerenigingContext context)
    {
        return new()
        {
            Beschrijving = "Adres niet uniek in het adressenregister.",
            Gebeurtenis = nameof(AdresNietUniekInAdressenregister),
            Data = new AdresNietUniekInAdressenregister(
                context.ResultingVCode,
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

    private static HistoriekGebeurtenisResponse AdresKonNietOvergenomenWorden(RegistreerVerenigingContext context)
        => new()
        {
            Beschrijving = "Adres kon niet gevonden worden in het adressenregister.",
            Gebeurtenis = nameof(AdresWerdNietGevondenInAdressenregister),
            Data = new
            {
                vCode = context.ResultingVCode,
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
