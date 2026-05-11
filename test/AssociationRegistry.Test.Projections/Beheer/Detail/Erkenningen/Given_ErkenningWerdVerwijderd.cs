namespace AssociationRegistry.Test.Projections.Beheer.Detail.Erkenningen;

using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerwijderd(BeheerDetailScenarioFixture<ErkenningWerdVerwijderdScenario> fixture)
    : BeheerDetailScenarioClassFixture<ErkenningWerdVerwijderdScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Then_Erkenning_Is_Removed()
    {
        fixture.Result.Erkenningen.Should().BeEmpty();
    }
}
