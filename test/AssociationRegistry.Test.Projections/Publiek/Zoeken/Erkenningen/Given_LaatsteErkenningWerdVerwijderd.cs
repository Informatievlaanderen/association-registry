namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_LaatsteErkenningWerdVerwijderd(
    PubliekZoekenScenarioFixture<VzerMetErkenningenWerdLaatsteErkenningVerwijderdScenario> fixture
) : PubliekZoekenScenarioClassFixture<VzerMetErkenningenWerdLaatsteErkenningVerwijderdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var erkenning = fixture.Scenario.TeVerwijderenActieveErkenningWerdVerwijderd;
        var erkenningId = erkenning.ErkenningId;
        var actual = fixture.Result.Erkenningen.Where(x => x.Key == erkenningId).ToList();

        actual.Should().NotContainKey(erkenningId);
    }

    [Fact]
    public void IsErkend_Is_True()
    {
        fixture.Result.IsErkend.Should().BeTrue();
    }
}
