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
                    Startdatum = fixture.Scenario.ErkenningWerdGecorrigeerd.Startdatum,
                    Einddatum = fixture.Scenario.ErkenningWerdGecorrigeerd.Einddatum,
                    Hernieuwingsdatum = fixture.Scenario.ErkenningWerdGecorrigeerd.Hernieuwingsdatum,
                    HernieuwingsUrl = fixture.Scenario.ErkenningWerdGecorrigeerd.HernieuwingsUrl,
                    Status = fixture.Scenario.ErkenningWerdGecorrigeerd.Status,
                    RedenSchorsing = string.Empty,
                },
                config: options => options.Excluding(x => x.JsonLdMetadata)
            );
}
