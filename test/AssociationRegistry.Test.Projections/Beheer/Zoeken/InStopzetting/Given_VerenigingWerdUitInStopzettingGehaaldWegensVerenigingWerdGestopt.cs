namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.InStopzetting;

using Scenario.InStopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestopt(
    BeheerZoekenScenarioFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestoptScenario> fixture
) : BeheerZoekenScenarioClassFixture<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestoptScenario>
{
    [Fact]
    public void InStopzetting_Is_True()
    {
        fixture.Result.InStopzetting.Should().BeFalse();
    }
}
