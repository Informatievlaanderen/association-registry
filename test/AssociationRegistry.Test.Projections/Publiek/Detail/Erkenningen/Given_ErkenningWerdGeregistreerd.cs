namespace AssociationRegistry.Test.Projections.Publiek.Detail.Erkenningen;

using Public.Schema.Detail;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGeregistreerd(PubliekDetailScenarioFixture<ErkenningWerdGeregistreerdScenario> fixture)
    : PubliekDetailScenarioClassFixture<ErkenningWerdGeregistreerdScenario>
{
    [Fact]
    public void Document_Erkenning_Werd_Geregistreerd() =>
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
                    Startdatum = fixture.Scenario.ErkenningWerdGeregistreerd.Startdatum,
                    Einddatum = fixture.Scenario.ErkenningWerdGeregistreerd.Einddatum,
                    Hernieuwingsdatum = fixture.Scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum,
                    HernieuwingsUrl = fixture.Scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                    RedenSchorsing = string.Empty,
                    Status = fixture.Scenario.ErkenningWerdGeregistreerd.Status,
                },
                config: options => options.Excluding(x => x.JsonLdMetadata)
            );
}
