namespace AssociationRegistry.Test.Projections.Beheer.Detail.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerlopen(BeheerDetailScenarioFixture<ErkenningWerdVerlopenScenario> fixture)
    : BeheerDetailScenarioClassFixture<ErkenningWerdVerlopenScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Then_Status_Is_Verlopen()
    {
        fixture.Result.Erkenningen.Single().Status.Should().Be(ErkenningStatus.Verlopen.Value);
    }
}
