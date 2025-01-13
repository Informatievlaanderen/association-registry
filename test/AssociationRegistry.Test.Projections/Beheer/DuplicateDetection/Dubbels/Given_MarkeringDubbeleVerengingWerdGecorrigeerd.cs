namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_MarkeringDubbeleVerengingWerdGecorrigeerd(DuplicateDetectionScenarioFixture<MarkeringDubbeleVerengingWerdGecorrigeerdScenario> fixture)
    : DuplicateDetectionClassFixture<MarkeringDubbeleVerengingWerdGecorrigeerdScenario>
{
    [Fact]
    public void Status_Is_Dubbel()
        => fixture.Result.IsDubbel.Should().BeFalse();
}
