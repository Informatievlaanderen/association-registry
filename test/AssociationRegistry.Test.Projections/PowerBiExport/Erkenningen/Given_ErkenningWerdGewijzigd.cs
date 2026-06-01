namespace AssociationRegistry.Test.Projections.PowerBiExport.Erkenningen;

using Admin.ProjectionHost.Constants;
using Scenario.Erkenningen;
using Erkenning = Admin.Schema.PowerBiExport.Erkenning;
using GegevensInitiator = Admin.Schema.PowerBiExport.GegevensInitiator;
using IpdcProduct = Admin.Schema.PowerBiExport.IpdcProduct;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGewijzigd(
    PowerBiScenarioFixture<ErkenningWerdGewijzigdScenario> fixture
) : PowerBiScenarioClassFixture<ErkenningWerdGewijzigdScenario>
{
    [Fact]
    public void Erkenning_Werd_Gecorrigeerd()
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
                    Startdatum = fixture.Scenario.ErkenningWerdGewijzigd.Startdatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Einddatum = fixture.Scenario.ErkenningWerdGewijzigd.Einddatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    Hernieuwingsdatum = fixture.Scenario.ErkenningWerdGewijzigd.Hernieuwingsdatum?.ToString(
                        WellknownFormats.DateOnly
                    ),
                    HernieuwingsUrl = fixture.Scenario.ErkenningWerdGewijzigd.HernieuwingsUrl,
                    RedenSchorsing = string.Empty,
                    Status = fixture.Scenario.ErkenningWerdGewijzigd.Status,
                },
            ]);
    }
}
