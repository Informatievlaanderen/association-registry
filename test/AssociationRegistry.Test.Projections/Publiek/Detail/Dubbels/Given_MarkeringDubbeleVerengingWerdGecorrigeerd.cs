namespace AssociationRegistry.Test.Projections.Publiek.Detail.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_MarkeringDubbeleVerengingWerdGecorrigeerd(PubliekDetailScenarioFixture<MarkeringDubbeleVerengingWerdGecorrigeerdScenario> fixture)
    : PubliekDetailScenarioClassFixture<MarkeringDubbeleVerengingWerdGecorrigeerdScenario>
{
    [Fact]
    public void Document_Status_Is_Dubbel()
        => fixture.Result.Status.Should().Be(fixture.Scenario.MarkeringDubbeleVerengingWerdGecorrigeerd.VorigeStatus);
}
