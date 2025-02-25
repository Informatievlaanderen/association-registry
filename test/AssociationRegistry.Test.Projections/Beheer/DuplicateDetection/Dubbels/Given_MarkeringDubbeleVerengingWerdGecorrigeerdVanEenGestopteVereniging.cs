namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Dubbels;

using Scenario.Dubbels;

[Collection(nameof(ProjectionContext))]
public class MarkeringDubbeleVerengingWerdGecorrigeerdVanEenGestopteVereniging(DuplicateDetectionScenarioFixture<MarkeringDubbeleVerengingWerdGecorrigeerdMetVorigeStatusGestoptScenario> fixture)
    : DuplicateDetectionClassFixture<MarkeringDubbeleVerengingWerdGecorrigeerdMetVorigeStatusGestoptScenario>
{
    [Fact]
    public void Status_Is_Gestopt()
        => fixture.Result.IsGestopt.Should().BeTrue();
}
