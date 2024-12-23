namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGemarkeerdAlsDubbel(DuplicateDetectionScenarioFixture<VerenigingWerdGemarkeerdAlsDubbelVanScenario> fixture)
    : DuplicateDetectionClassFixture<VerenigingWerdGemarkeerdAlsDubbelVanScenario>
{
    [Fact]
    public void Status_Is_Dubbel()
        => fixture.Result.IsDubbel.Should().BeTrue();
}
