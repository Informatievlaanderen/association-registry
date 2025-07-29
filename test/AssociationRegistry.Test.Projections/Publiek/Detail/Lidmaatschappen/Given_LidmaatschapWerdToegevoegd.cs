namespace AssociationRegistry.Test.Projections.Publiek.Detail.Lidmaatschappen;

using Public.Schema.Detail;
using Scenario.Lidmaatschappen;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdToegevoegd(PubliekDetailScenarioFixture<LidmaatschapWerdToegevoegdScenario> fixture)
    : PubliekDetailScenarioClassFixture<LidmaatschapWerdToegevoegdScenario>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Lidmaatschappen[0]
                  .Should()
                  .BeEquivalentTo(
                       new PubliekVerenigingDetailDocument.Types.Lidmaatschap(
                           JsonLdMetadata: null,
                           fixture.Scenario.LidmaatschapWerdToegevoegdFirst.Lidmaatschap.LidmaatschapId,
                           fixture.Scenario.LidmaatschapWerdToegevoegdFirst.Lidmaatschap.AndereVereniging,
                           fixture.Scenario.LidmaatschapWerdToegevoegdFirst.Lidmaatschap.DatumVan,
                           fixture.Scenario.LidmaatschapWerdToegevoegdFirst.Lidmaatschap.DatumTot,
                           fixture.Scenario.LidmaatschapWerdToegevoegdFirst.Lidmaatschap.Identificatie,
                           fixture.Scenario.LidmaatschapWerdToegevoegdFirst.Lidmaatschap.Beschrijving
                       ),
                       config: options => options.Excluding(x => x.JsonLdMetadata));
}
