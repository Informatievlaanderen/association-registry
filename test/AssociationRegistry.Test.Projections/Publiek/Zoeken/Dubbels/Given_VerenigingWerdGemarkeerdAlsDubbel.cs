namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Dubbels;

using Scenario.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGemarkeerdAlsDubbel(PubliekZoekenScenarioFixture<VerenigingWerdGemarkeerdAlsDubbelVanScenario> fixture)
    : PubliekZoekenScenarioClassFixture<VerenigingWerdGemarkeerdAlsDubbelVanScenario>
{
    [Fact]
    public void Status_Is_Dubbel()
        => fixture.Result.IsDubbel.Should().BeTrue();
}
