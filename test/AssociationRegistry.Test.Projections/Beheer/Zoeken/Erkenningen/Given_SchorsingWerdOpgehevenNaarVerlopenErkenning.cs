namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Erkenningen;

using AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;
using Publiek.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_SchorsingWerdOpgehevenNaarVerlopenErkenning(
    BeheerZoekenScenarioFixture<VzerMetActieveErkenningWerdOpgehevenNaarVerlopenScenario> fixture
) : BeheerZoekenScenarioClassFixture<VzerMetActieveErkenningWerdOpgehevenNaarVerlopenScenario>
{
    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
