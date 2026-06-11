namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerwijderd(PubliekZoekenScenarioFixture<VzerMetErkenningWerdVerwijderdScenario> fixture)
    : PubliekZoekenScenarioClassFixture<VzerMetErkenningWerdVerwijderdScenario>
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
