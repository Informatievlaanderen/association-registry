namespace AssociationRegistry.Test.Projections.PowerBiExport.Erkenningen;

using Admin.ProjectionHost.Constants;
using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen;
using Erkenning = Admin.Schema.PowerBiExport.Erkenning;
using GegevensInitiator = Admin.Schema.PowerBiExport.GegevensInitiator;
using IpdcProduct = Admin.Schema.PowerBiExport.IpdcProduct;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningRedenVanSchorsingWerdGecorrigeerd(
    PowerBiScenarioFixture<ErkenningRedenVanSchorsingWerdGecorrigeerdScenario> fixture
) : PowerBiScenarioClassFixture<ErkenningRedenVanSchorsingWerdGecorrigeerdScenario>
{
    [Fact]
    public void Erkenning_Werd_Geschorst()
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
                    RedenSchorsing = fixture.Scenario.ErkenningRedenVanSchorsingWerdGecorrigeerd.RedenSchorsing,
                    Status = ErkenningStatus.Geschorst,
                },
            ]);
    }
}
