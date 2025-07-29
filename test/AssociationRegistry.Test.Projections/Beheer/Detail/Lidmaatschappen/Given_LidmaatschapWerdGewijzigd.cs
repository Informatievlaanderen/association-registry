namespace AssociationRegistry.Test.Projections.Beheer.Detail.Lidmaatschappen;

using Admin.Schema.Detail;
using Scenario.Lidmaatschappen;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdGewijzigd(BeheerDetailScenarioFixture<LidmaatschapWerdGewijzigdScenario> fixture)
    : BeheerDetailScenarioClassFixture<LidmaatschapWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(4);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Lidmaatschappen[0]
                  .Should()
                  .BeEquivalentTo(
                       new Lidmaatschap(
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
