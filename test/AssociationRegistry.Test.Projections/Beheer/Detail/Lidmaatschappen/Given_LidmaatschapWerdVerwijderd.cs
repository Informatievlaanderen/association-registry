namespace AssociationRegistry.Test.Projections.Beheer.Detail.Lidmaatschappen;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdVerwijderd(BeheerDetailScenarioFixture<LidmaatschapWerdVerwijderdScenario> fixture)
    : BeheerDetailScenarioClassFixture<LidmaatschapWerdVerwijderdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Lidmaatschappen.Should().BeEmpty();
}
