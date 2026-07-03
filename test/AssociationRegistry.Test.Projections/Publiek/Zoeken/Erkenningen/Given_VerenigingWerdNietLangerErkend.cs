namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdNietLangerErkend(
    PubliekZoekenScenarioFixture<VerenigingWerdNietLangerErkendScenario> fixture
) : PubliekZoekenScenarioClassFixture<VerenigingWerdNietLangerErkendScenario>
{
    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
