namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Erkenningen;

using Scenario.Erkenningen;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdErkend(BeheerZoekenScenarioFixture<VerenigingWerdErkendScenario> fixture)
    : BeheerZoekenScenarioClassFixture<VerenigingWerdErkendScenario>
{
    [Fact]
    public void IsErkend_Is_True()
    {
        fixture.Result.IsErkend.Should().BeTrue();
    }
}
