namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_SchorsingWerdOpgehevenNaarVerlopenErkenning(
    PubliekZoekenScenarioFixture<VzerMetActieveErkenningWerdOpgehevenNaarVerlopenScenario> fixture
) : PubliekZoekenScenarioClassFixture<VzerMetActieveErkenningWerdOpgehevenNaarVerlopenScenario>
{
    [Fact]
    public void Document_Erkenningen_Are_Verlopen()
    {
        fixture
            .Result.Erkenningen.Should()
            .BeEquivalentTo(
                new Dictionary<int, string>()
                {
                    { fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId, ErkenningStatus.Verlopen.Value },
                }
            );
    }

    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
