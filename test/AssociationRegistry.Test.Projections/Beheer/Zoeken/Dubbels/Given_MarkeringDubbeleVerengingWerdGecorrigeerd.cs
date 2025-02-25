namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Dubbels;

using Scenario.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_MarkeringDubbeleVerengingWerdGecorrigeerd(BeheerZoekenScenarioFixture<MarkeringDubbeleVerengingWerdGecorrigeerdScenario> fixture)
    : BeheerZoekenScenarioClassFixture<MarkeringDubbeleVerengingWerdGecorrigeerdScenario>
{
    [Fact]
    public void Status_Is_Dubbel()
        => fixture.Result.IsDubbel.Should().BeFalse();
}
