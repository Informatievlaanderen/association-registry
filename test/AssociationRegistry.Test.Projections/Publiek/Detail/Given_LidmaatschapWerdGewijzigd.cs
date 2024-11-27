namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using Public.Schema.Detail;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdGewijzigd(LidmaatschapWerdGewijzigdFixture fixture) : IClassFixture<LidmaatschapWerdGewijzigdFixture>
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
