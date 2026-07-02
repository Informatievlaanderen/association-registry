namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Erkenningen;

using AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;
using Publiek.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ActieveErkenningWerdGeregistreerd(
    BeheerZoekenScenarioFixture<VzerMetActieveErkenningWerdGeregistreerdScenario> fixture
) : BeheerZoekenScenarioClassFixture<VzerMetActieveErkenningWerdGeregistreerdScenario>
{
    [Fact]
    public void IsErkend_Is_True()
    {
        fixture.Result.IsErkend.Should().BeTrue();
    }
}
