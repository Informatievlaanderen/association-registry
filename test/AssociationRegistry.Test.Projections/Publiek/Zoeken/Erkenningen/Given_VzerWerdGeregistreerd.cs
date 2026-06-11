namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_VzerWerdGeregistreerd(
    PubliekZoekenScenarioFixture<VzerZonderErkenningWerdGeregistreerdScenario> fixture
) : PubliekZoekenScenarioClassFixture<VzerZonderErkenningWerdGeregistreerdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var erkenning = fixture.Scenario.ErkenningWerdGeregistreerd;
        var erkenningId = erkenning.ErkenningId;
        fixture.Result.Erkenningen.Where(x => x.Key == erkenningId).Should().BeNullOrEmpty();
    }

    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
