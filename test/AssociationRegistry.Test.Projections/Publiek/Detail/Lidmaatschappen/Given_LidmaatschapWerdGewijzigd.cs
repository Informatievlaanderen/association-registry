namespace AssociationRegistry.Test.Projections.Publiek.Detail.Lidmaatschappen;

using Public.Schema.Detail;
using Scenario.Lidmaatschappen;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdGewijzigd(PubliekDetailScenarioFixture<LidmaatschapWerdGewijzigdScenario> fixture)
    : PubliekDetailScenarioClassFixture<LidmaatschapWerdGewijzigdScenario>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Lidmaatschappen[0]
                  .Should()
                  .BeEquivalentTo(
                       new PubliekVerenigingDetailDocument.Lidmaatschap(
                           JsonLdMetadata: null,
                           fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.LidmaatschapId,
                           fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.AndereVereniging,
                           fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.DatumVan,
                           fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.DatumTot,
                           fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.Identificatie,
                           fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.Beschrijving
                       ),
                       config: options => options.Excluding(x => x.JsonLdMetadata));
}
