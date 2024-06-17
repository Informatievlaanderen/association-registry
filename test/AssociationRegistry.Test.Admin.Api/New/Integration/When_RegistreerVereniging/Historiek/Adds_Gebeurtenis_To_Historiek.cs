namespace AssociationRegistry.Test.Admin.Api.New.Historiek;

using Alba;
using AssociationRegistry.Admin.Api.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using Events;
using FluentAssertions;
using Framework;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using templates;
using Vereniging;
using Xunit;

[Collection(nameof(RegistreerVerenigingContext))]
public class Adds_Gebeurtenis_To_Historiek: RegistreerVerenigingContext
{
    private readonly AppFixture _fixture;
    private readonly IAlbaHost theHost;

    public Adds_Gebeurtenis_To_Historiek(AppFixture fixture) : base(fixture)
    {
        _fixture = fixture;
        theHost = fixture.Host;
    }


        [Fact]
        public async Task JsonContentMatches()
        {
            var result = await theHost.GetAsJson<HistoriekResponse>(url: $"/v1/verenigingen/{ResultingVCode}/historiek");

            var comparisonConfig = new ComparisonConfig();
            comparisonConfig.MaxDifferences = 10;
            comparisonConfig.CustomPropertyComparer<HistoriekGebeurtenisResponse>(x => x.Data, new JObjectComparer(RootComparerFactory.GetRootComparer()));
            comparisonConfig.IgnoreProperty<FeitelijkeVerenigingWerdGeregistreerdData>(x => x.Vertegenwoordigers);
            comparisonConfig.IgnoreProperty<FeitelijkeVerenigingWerdGeregistreerdData>(x => x.HoofdactiviteitenVerenigingsloket);
            comparisonConfig.IgnoreProperty<HistoriekGebeurtenisResponse>(x => x.Tijdstip);
            comparisonConfig.MaxMillisecondsDateDifference = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

            result.ShouldCompare(expected: new HistoriekResponse()
            {
                Context = "http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json",
                VCode = ResultingVCode,
                Gebeurtenissen = new HistoriekGebeurtenisResponse[]
                {
                    new()
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
                                                             i+1,
                                                             x.Contactgegeventype,
                                                             x.Waarde,
                                                             x.Beschrijving,
                                                             x.IsPrimair))
                                                    .ToArray(),
                            Locaties: Request.Locaties.Select(
                                                  (x, i) => new Registratiedata.Locatie(
                                                      i+1,
                                                      x.Locatietype,
                                                      x.IsPrimair,
                                                      x.Naam,
                                                      x.Adres == null? null : new Registratiedata.Adres(
                                                          x.Adres.Straatnaam,
                                                          x.Adres.Huisnummer,
                                                          x.Adres.Busnummer,
                                                          x.Adres.Postcode,
                                                          x.Adres.Gemeente,
                                                          x.Adres.Land
                                                      ),
                                                      x.AdresId == null? null :new Registratiedata.AdresId(x.AdresId.Broncode, x.AdresId.Bronwaarde)))
                                             .ToArray(),
                            Vertegenwoordigers: null,
                            HoofdactiviteitenVerenigingsloket: null),
                        Initiator = AppFixture.Initiator,
                    }
                },
            }, compareConfig: comparisonConfig);
        }

        private JObject ToJObject(FeitelijkeVerenigingWerdGeregistreerdData feitelijkeVerenigingWerdGeregistreerdData)
        {
            var geregistreerdDataJson = JsonConvert.SerializeObject(feitelijkeVerenigingWerdGeregistreerdData);

            return JObject.Parse(geregistreerdDataJson);
        }
}

