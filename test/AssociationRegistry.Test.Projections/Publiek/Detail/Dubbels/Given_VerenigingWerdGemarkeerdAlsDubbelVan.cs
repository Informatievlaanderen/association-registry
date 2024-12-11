namespace AssociationRegistry.Test.Projections.Publiek.Detail.Dubbels;

using Public.Schema.Constants;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGemarkeerdAlsDubbelVan(PubliekDetailScenarioFixture<VerenigingWerdGemarkeerdAlsDubbelVanScenario> fixture)
    : PubliekDetailScenarioClassFixture<VerenigingWerdGemarkeerdAlsDubbelVanScenario>
{
    [Fact]
    public void Document_IsDubbelVan_Is_Updated()
        => fixture.Result.IsDubbelVan.Should().Be(fixture.Scenario.VerenigingWerdGermarkeerdAlsDubbelVan.VCodeAuthentiekeVereniging);

    [Fact]
    public void Document_Status_Is_Dubbel()
        => fixture.Result.Status.Should().Be(VerenigingStatus.Dubbel);
}
