namespace AssociationRegistry.Test.Projections.Publiek.Detail.Erkenningen;

using AssociationRegistry.Public.Schema.Detail;
using AssociationRegistry.Test.Projections.Scenario.Lidmaatschappen;
using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGeregistreerd(PubliekDetailScenarioFixture<ErkenningWerdGeregistreerdScenario> fixture)
    : PubliekDetailScenarioClassFixture<ErkenningWerdGeregistreerdScenario>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Erkenningen[0]
                  .Should()
                  .BeEquivalentTo(
                       new PubliekVerenigingDetailDocument.Types.Erkenning
                       {
                           JsonLdMetadata = null,
                           ErkenningId = fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                           VCode = fixture.Scenario.ErkenningWerdGeregistreerd.VCode,
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
                           Motivering = string.Empty,
                           Status = ErkenningStatus.Create(fixture.Scenario.ErkenningWerdGeregistreerd.Startdatum,
                                                           fixture.Scenario.ErkenningWerdGeregistreerd.Einddatum).Value,
                       },
                       config: options => options.Excluding(x => x.JsonLdMetadata));
}
