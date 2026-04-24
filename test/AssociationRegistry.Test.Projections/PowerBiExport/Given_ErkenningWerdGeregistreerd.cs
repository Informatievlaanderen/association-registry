namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.ProjectionHost.Constants;
using Admin.Schema.PowerBiExport;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGeregistreerd(PowerBiScenarioFixture<ErkenningWerdGeregistreerdScenario> fixture)
    : PowerBiScenarioClassFixture<ErkenningWerdGeregistreerdScenario>
{
    [Fact]
    public void Erkenning_Werd_Geregistreerd()
    {
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
                    Einddatum = fixture.Scenario.ErkenningWerdGeregistreerd.Einddatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Hernieuwingsdatum = fixture.Scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    HernieuwingsUrl = fixture.Scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                    RedenSchorsing = string.Empty,
                    Status = fixture.Scenario.ErkenningWerdGeregistreerd.Status,
                },
            ]);
    }
}
