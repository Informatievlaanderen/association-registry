namespace AssociationRegistry.Test.Projections.Publiek.Detail.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Public.Schema.Detail;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGecorrigeerd(
    PubliekDetailScenarioFixture<ErkenningWerdGecorrigeerdScenario> fixture
) : PubliekDetailScenarioClassFixture<ErkenningWerdGecorrigeerdScenario>
{
    [Fact]
    public void Document_Is_Updated() =>
        fixture
           .Result.Erkenningen.First()
           .Should()
           .BeEquivalentTo(
                new PubliekVerenigingDetailDocument.Types.Erkenning
                {
                    JsonLdMetadata = null,
                    ErkenningId = fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                    GeregistreerdDoor = new()
                    {
                        OvoCode = fixture.Scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
                        Naam = fixture.Scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.Naam,
                    },
                    IpdcProduct = new()
                    {
                        Nummer = fixture.Scenario.ErkenningWerdGeregistreerd.IpdcProduct.Nummer,
                        Naam = fixture.Scenario.ErkenningWerdGeregistreerd.IpdcProduct.Naam,
                    },
                    Startdatum = fixture.Scenario.ErkenningWerdGewijzigd.Startdatum,
                    Einddatum = fixture.Scenario.ErkenningWerdGewijzigd.Einddatum,
                    Hernieuwingsdatum = fixture.Scenario.ErkenningWerdGewijzigd.Hernieuwingsdatum,
                    HernieuwingsUrl = fixture.Scenario.ErkenningWerdGewijzigd.HernieuwingsUrl,
                    Status = fixture.Scenario.ErkenningWerdGewijzigd.Status,
                    RedenSchorsing = string.Empty,
                },
                config: options => options.Excluding(x => x.JsonLdMetadata)
            );
}
