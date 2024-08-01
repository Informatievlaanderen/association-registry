namespace AssociationRegistry.Test.E2E.When_Registreer_Vereniging.Beheer_Historiek;

using Alba;
using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Schema.Historiek.EventData;
using Events;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

[Collection(nameof(RegistreerVerenigingContext))]
public class Adds_WerdGeregistreerd_Gebeurtenis_To_Historiek : RegistreerVerenigingContext
{
    private readonly IAlbaHost theHost;

    public Adds_WerdGeregistreerd_Gebeurtenis_To_Historiek(AppFixture fixture) : base(fixture)
    {
        theHost = fixture.Host;
    }

    [Fact(Skip = "Not now")]
    public async Task JsonContentMatches()
    {
        var result = await theHost.GetAsJson<HistoriekResponse>(url: $"/v1/verenigingen/{ResultingVCode}/historiek");

        var comparisonConfig = new ComparisonConfig();
        comparisonConfig.MaxDifferences = 10;

        comparisonConfig.CustomPropertyComparer<HistoriekGebeurtenisResponse>(
            x => x.Data, new JObjectComparer(RootComparerFactory.GetRootComparer()));

        comparisonConfig.IgnoreProperty<FeitelijkeVerenigingWerdGeregistreerdData>(x => x.Vertegenwoordigers);
        comparisonConfig.IgnoreProperty<FeitelijkeVerenigingWerdGeregistreerdData>(x => x.HoofdactiviteitenVerenigingsloket);
        comparisonConfig.IgnoreProperty<HistoriekGebeurtenisResponse>(x => x.Tijdstip);
        comparisonConfig.MaxMillisecondsDateDifference = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

        result.Gebeurtenissen[1].ShouldCompare(expected: new HistoriekGebeurtenisResponse
        {
            Beschrijving = "Adres werd overgenomen uit het adressenregister.",
            Gebeurtenis = nameof(AdresWerdOvergenomenUitAdressenregister),
            Data = new AdresWerdOvergenomenUitAdressenregister(
                VCode: ResultingVCode,
                LocatieId: 1,
                Adres: new Registratiedata.AdresUitAdressenregister(
                    "Leopold II-laan",
                    "99",
                    "",
                    "9200",
                    "Dendermonde"),
                AdresId: new Registratiedata.AdresId("AR", "https://data.vlaanderen.be/id/adres/3213019")),
            Initiator = EventStore.EventStore.DigitaalVlaanderenOvoNumber,
        }, compareConfig: comparisonConfig);

        result.Gebeurtenissen[2].ShouldCompare(expected: new HistoriekGebeurtenisResponse
        {
            Beschrijving = "Adres niet uniek in het adressenregister.",
            Gebeurtenis = nameof(AdresNietUniekInAdressenregister),
            Data = new
            {
                vCode = ResultingVCode,
                locatieId = 2,
                nietOvergenomenAdressenUitAdressenregister = new List<object>
                {
                    new
                    {
                        adresId = new { broncode = "AR", bronwaarde = "https://data.vlaanderen.be/id/adres/3213019" },
                        adresvoorstelling = "Leopold II-laan 99, 9200 Dendermonde",
                        score = 95.4942299939528,
                    },
                    new
                    {
                        adresId = new { broncode = "AR", bronwaarde = "https://data.vlaanderen.be/id/adres/20055852" },
                        adresvoorstelling = "Leopold II-laan 99 bus 1, 9200 Dendermonde",
                        score = 86.987355612507,
                    },
                    new
                    {
                        adresId = new { broncode = "AR", bronwaarde = "https://data.vlaanderen.be/id/adres/5459542" },
                        adresvoorstelling = "Leopold II-laan 99 bus 2, 9200 Dendermonde",
                        score = 86.987355612507,
                    },
                    new
                    {
                        adresId = new { broncode = "AR", bronwaarde = "https://data.vlaanderen.be/id/adres/5556720" },
                        adresvoorstelling = "Leopold II-laan 99 bus 3, 9200 Dendermonde",
                        score = 86.987355612507,
                    },
                    new
                    {
                        adresId = new { broncode = "AR", bronwaarde = "https://data.vlaanderen.be/id/adres/5435137" },
                        adresvoorstelling = "Leopold II-laan 99 bus 6, 9200 Dendermonde",
                        score = 86.987355612507,
                    },
                    new
                    {
                        adresId = new { broncode = "AR", bronwaarde = "https://data.vlaanderen.be/id/adres/5451858" },
                        adresvoorstelling = "Leopold II-laan 99 bus 7, 9200 Dendermonde",
                        score = 86.987355612507,
                    },
                    new
                    {
                        adresId = new { broncode = "AR", bronwaarde = "https://data.vlaanderen.be/id/adres/19038299" },
                        adresvoorstelling = "Leopold II-laan 99 bus 8, 9200 Dendermonde",
                        score = 86.987355612507,
                    },
                    new
                    {
                        adresId = new { broncode = "AR", bronwaarde = "https://data.vlaanderen.be/id/adres/20172048" },
                        adresvoorstelling = "Leopold II-laan 99 bus 9, 9200 Dendermonde",
                        score = 86.987355612507,
                    },
                    new
                    {
                        adresId = new { broncode = "AR", bronwaarde = "https://data.vlaanderen.be/id/adres/5656904" },
                        adresvoorstelling = "Leopold II-laan 99 bus 10, 9200 Dendermonde",
                        score = 86.987355612507,
                    },
                    new
                    {
                        adresId = new { broncode = "AR", bronwaarde = "https://data.vlaanderen.be/id/adres/5512213" },
                        adresvoorstelling = "Leopold II-laan 99 bus 11, 9200 Dendermonde",
                        score = 86.987355612507,
                    },
                },
            },
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:04Z",
        }, compareConfig: comparisonConfig);

        result.Gebeurtenissen[3].ShouldCompare(expected: new HistoriekGebeurtenisResponse
        {
            Beschrijving = "Adres kon niet gevonden worden in het adressenregister.",
            Gebeurtenis = nameof(AdresWerdNietGevondenInAdressenregister),
            Data = new
            {
                vCode = ResultingVCode,
                locatieId = 3,
                straatnaam = "dorpelstraat",
                huisnummer = "169",
                busnummer = "2",
                postcode = "4567",
                gemeente = "Nothingham",
            },
            Initiator = "OVO002949",
            Tijdstip = "2024-07-30T11:08:05Z",
        }, compareConfig: comparisonConfig);

    }

    private JObject ToJObject(FeitelijkeVerenigingWerdGeregistreerdData feitelijkeVerenigingWerdGeregistreerdData)
    {
        var geregistreerdDataJson = JsonConvert.SerializeObject(feitelijkeVerenigingWerdGeregistreerdData);

        return JObject.Parse(geregistreerdDataJson);
    }
}
