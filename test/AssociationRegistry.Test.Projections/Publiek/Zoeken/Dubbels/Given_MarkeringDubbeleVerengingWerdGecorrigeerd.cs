namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Dubbels;

using Scenario.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_MarkeringDubbeleVerengingWerdGecorrigeerd(PubliekZoekenScenarioFixture<MarkeringDubbeleVerengingWerdGecorrigeerdScenario> fixture)
    : PubliekZoekenScenarioClassFixture<MarkeringDubbeleVerengingWerdGecorrigeerdScenario>
{
    [Fact]
    public void Status_Is_Dubbel()
        => fixture.Result.IsDubbel.Should().BeFalse();
}
