namespace AssociationRegistry.Test.Projections.Beheer.Detail.Lidmaatschappen;

using Admin.Schema.Detail;
using Scenario.Lidmaatschappen;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdToegevoegd(BeheerDetailScenarioFixture<LidmaatschapWerdToegevoegdScenario> fixture)
    : BeheerDetailScenarioClassFixture<LidmaatschapWerdToegevoegdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Lidmaatschappen[0]
                  .Should()
                  .BeEquivalentTo(
                       new Lidmaatschap(
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
