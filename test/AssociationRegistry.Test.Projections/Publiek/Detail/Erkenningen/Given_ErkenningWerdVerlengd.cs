namespace AssociationRegistry.Test.Projections.Publiek.Detail.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Public.Schema.Detail;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerlengd(PubliekDetailScenarioFixture<ErkenningWerdVerlengdScenario> fixture)
    : PubliekDetailScenarioClassFixture<ErkenningWerdVerlengdScenario>
{
    [Fact]
    public void Document_Is_Updated()
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
                    Einddatum = fixture.Scenario.ErkenningWerdVerlengd.Einddatum,
                    Hernieuwingsdatum = fixture.Scenario.ErkenningWerdVerlengd.Hernieuwingsdatum,
                    HernieuwingsUrl = fixture.Scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                    RedenSchorsing = string.Empty,
                    Status = status.Value,
                },
                config: options => options.Excluding(x => x.JsonLdMetadata)
            );
    }
}
