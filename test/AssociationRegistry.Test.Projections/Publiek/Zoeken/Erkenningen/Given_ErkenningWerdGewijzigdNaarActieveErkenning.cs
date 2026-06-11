namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGewijzigdNaarActieveErkenning(
    PubliekZoekenScenarioFixture<VzerMetGeregistreerdeErkenningWordtGewijzigdNaarActiefScenario> fixture
) : PubliekZoekenScenarioClassFixture<VzerMetGeregistreerdeErkenningWordtGewijzigdNaarActiefScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        fixture
            .Result.Erkenningen.Should()
            .BeEquivalentTo(
                new Dictionary<int, string>()
                {
                    { fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId, ErkenningStatus.Actief.Value },
                }
            );
    }

    [Fact]
    public void IsErkend_Is_True()
    {
        fixture.Result.IsErkend.Should().BeTrue();
    }
}
