namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdErkend(PubliekZoekenScenarioFixture<VerenigingWerdErkendScenario> fixture)
    : PubliekZoekenScenarioClassFixture<VerenigingWerdErkendScenario>
{
    [Fact]
    public void IsErkend_Is_True()
    {
        fixture.Result.IsErkend.Should().BeTrue();
    }
}
