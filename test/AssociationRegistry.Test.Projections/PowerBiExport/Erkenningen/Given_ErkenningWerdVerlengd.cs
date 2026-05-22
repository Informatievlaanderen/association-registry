namespace AssociationRegistry.Test.Projections.PowerBiExport.Erkenningen;

using Admin.ProjectionHost.Constants;
using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen;
using Erkenning = Admin.Schema.PowerBiExport.Erkenning;
using GegevensInitiator = Admin.Schema.PowerBiExport.GegevensInitiator;
using IpdcProduct = Admin.Schema.PowerBiExport.IpdcProduct;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerlengd(PowerBiScenarioFixture<ErkenningWerdVerlengdScenario> fixture)
    : PowerBiScenarioClassFixture<ErkenningWerdVerlengdScenario>
{
    [Fact]
    public void Erkenning_Werd_Geschorst()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var status = ErkenningStatus.Bepaal(
            ErkenningsPeriode.Create(
                fixture.Scenario.ErkenningWerdGeregistreerd.Startdatum,
                fixture.Scenario.ErkenningWerdVerlengd.Einddatum
            ),
            today
        );

        fixture
            .Result.Erkenningen.Should()
            .BeEquivalentTo([
                new Erkenning
                {
                    ErkenningId = fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                    GeregistreerdDoor = new GegevensInitiator
                    {
                        OvoCode = fixture.Scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
                        Naam = fixture.Scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.Naam,
                    },
                    IpdcProduct = new IpdcProduct
                    {
                        Nummer = fixture.Scenario.ErkenningWerdGeregistreerd.IpdcProduct.Nummer,
                        Naam = fixture.Scenario.ErkenningWerdGeregistreerd.IpdcProduct.Naam,
                    },
                    Startdatum = fixture.Scenario.ErkenningWerdGeregistreerd.Startdatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Einddatum = fixture.Scenario.ErkenningWerdVerlengd.Einddatum.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Hernieuwingsdatum = fixture.Scenario.ErkenningWerdVerlengd.Hernieuwingsdatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    HernieuwingsUrl = fixture.Scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                    RedenSchorsing = string.Empty,
                    Status = status.Value,
                },
            ]);
    }
}
