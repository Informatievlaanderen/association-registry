namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Dubbels;

using Scenario.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGemarkeerdAlsDubbel(BeheerZoekenScenarioFixture<VerenigingWerdGemarkeerdAlsDubbelVanScenario> fixture)
    : BeheerZoekenScenarioClassFixture<VerenigingWerdGemarkeerdAlsDubbelVanScenario>
{
    [Fact]
    public void Status_Is_Dubbel()
        => fixture.Result.IsDubbel.Should().BeTrue();
}
