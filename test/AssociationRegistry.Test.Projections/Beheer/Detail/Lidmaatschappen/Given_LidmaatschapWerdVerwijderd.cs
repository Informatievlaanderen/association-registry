namespace AssociationRegistry.Test.Projections.Beheer.Detail.Lidmaatschappen;

using Scenario.Lidmaatschappen;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdVerwijderd(BeheerDetailScenarioFixture<LidmaatschapWerdVerwijderdScenario> fixture)
    : BeheerDetailScenarioClassFixture<LidmaatschapWerdVerwijderdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(4);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Lidmaatschappen.Should().NotContain(x => x.LidmaatschapId == fixture.Scenario.LidmaatschapWerdVerwijderd.Lidmaatschap.LidmaatschapId);
}
