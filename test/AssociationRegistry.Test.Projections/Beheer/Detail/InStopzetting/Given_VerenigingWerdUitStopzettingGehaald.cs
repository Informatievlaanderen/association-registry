namespace AssociationRegistry.Test.Projections.Beheer.Detail.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitStopzettingGehaald(
    BeheerDetailScenarioFixture<VerenigingWerdUitStopzettingGehaaldScenario> fixture
) : BeheerDetailScenarioClassFixture<VerenigingWerdUitStopzettingGehaaldScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.InStopzetting.Should().BeFalse();
    }
}
