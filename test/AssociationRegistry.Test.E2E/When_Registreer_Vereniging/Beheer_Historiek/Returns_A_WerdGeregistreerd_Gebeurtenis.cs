namespace AssociationRegistry.Test.E2E.When_Registreer_Vereniging.Beheer_Historiek;

using Alba;
using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Schema.Historiek.EventData;
using Events;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

[Collection(nameof(RegistreerVerenigingContext))]
public class Adds_Document : RegistreerVerenigingContext
{
    private readonly AppFixture _fixture;
    private readonly IAlbaHost theHost;

    public Adds_Document(AppFixture fixture) : base(fixture)
    {
        _fixture = fixture;
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

        result.Gebeurtenissen[0].ShouldCompare(expected: new HistoriekGebeurtenisResponse
            {
                    Beschrijving = $"Feitelijke vereniging werd geregistreerd met naam '{Request.Naam}'.",
                    Gebeurtenis = nameof(FeitelijkeVerenigingWerdGeregistreerd),
                    Data = new FeitelijkeVerenigingWerdGeregistreerdData(
                        VCode: ResultingVCode,
                        Naam: Request.Naam,
                        KorteNaam: Request.KorteNaam,
                        KorteBeschrijving: Request.KorteBeschrijving,
                        Startdatum: Request.Startdatum,
                        Doelgroep: new Registratiedata.Doelgroep(
                            Request.Doelgroep.Minimumleeftijd.Value,
                            Request.Doelgroep.Maximumleeftijd.Value),
                        IsUitgeschrevenUitPubliekeDatastroom: Request
                           .IsUitgeschrevenUitPubliekeDatastroom,
                        Contactgegevens: Request.Contactgegevens.Select(
                                                     (x, i) => new Registratiedata.Contactgegeven(
                                                         i + 1,
                                                         x.Contactgegeventype,
                                                         x.Waarde,
                                                         x.Beschrijving,
                                                         x.IsPrimair))
                                                .ToArray(),
                        Locaties: Request.Locaties.Select(
                                              (x, i) => new Registratiedata.Locatie(
                                                  i + 1,
                                                  x.Locatietype,
                                                  x.IsPrimair,
                                                  x.Naam,
                                                  x.Adres == null
                                                      ? null
                                                      : new Registratiedata.Adres(
                                                          x.Adres.Straatnaam,
                                                          x.Adres.Huisnummer,
                                                          x.Adres.Busnummer,
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
        }, compareConfig: comparisonConfig);
    }

    private JObject ToJObject(FeitelijkeVerenigingWerdGeregistreerdData feitelijkeVerenigingWerdGeregistreerdData)
    {
        var geregistreerdDataJson = JsonConvert.SerializeObject(feitelijkeVerenigingWerdGeregistreerdData);

        return JObject.Parse(geregistreerdDataJson);
    }
}
