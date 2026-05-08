namespace AssociationRegistry.Test.Projections.Publiek.Detail.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Public.Schema.Detail;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_SchorsingVanErkenningWerdOpgeheven(
    PubliekDetailScenarioFixture<SchorsingVanErkenningWerdOpgehevenScenario> fixture
) : PubliekDetailScenarioClassFixture<SchorsingVanErkenningWerdOpgehevenScenario>
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
                    Startdatum = fixture.Scenario.ErkenningWerdGeregistreerd.Startdatum,
                    Einddatum = fixture.Scenario.ErkenningWerdGeregistreerd.Einddatum,
                    Hernieuwingsdatum = fixture.Scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum,
                    HernieuwingsUrl = fixture.Scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                    RedenSchorsing = string.Empty,
                    Status = fixture.Scenario.SchorsingVanErkenningWerdOpgeheven.Status,
                },
                config: options => options.Excluding(x => x.JsonLdMetadata)
            );
}
