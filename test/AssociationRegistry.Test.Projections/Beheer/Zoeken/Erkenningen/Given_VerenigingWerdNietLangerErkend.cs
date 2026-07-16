namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Erkenningen;

using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdNietLangerErkend(
    BeheerZoekenScenarioFixture<VerenigingWerdNietLangerErkendScenario> fixture
) : BeheerZoekenScenarioClassFixture<VerenigingWerdNietLangerErkendScenario>
{
    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
