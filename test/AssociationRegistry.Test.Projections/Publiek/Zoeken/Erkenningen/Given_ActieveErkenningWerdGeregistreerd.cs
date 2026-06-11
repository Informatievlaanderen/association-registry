namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ActieveErkenningWerdGeregistreerd(
    PubliekZoekenScenarioFixture<VzerMetActieveErkenningWerdGeregistreerdScenario> fixture
) : PubliekZoekenScenarioClassFixture<VzerMetActieveErkenningWerdGeregistreerdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var erkenning = fixture.Scenario.ErkenningWerdGeregistreerd;
        var erkenningId = erkenning.ErkenningId;
        var actual = fixture.Result.Erkenningen.First(x => x.Key == erkenningId);

        actual.Value.Should().BeEquivalentTo(ErkenningStatus.Actief.Value);
    }

    [Fact]
    public void IsErkend_Is_True()
    {
        fixture.Result.IsErkend.Should().BeTrue();
    }
}
