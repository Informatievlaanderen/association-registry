namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ActieveErkenningWerdGeschorst(
    PubliekZoekenScenarioFixture<VzerMetActieveErkenningWerdGeschorstScenario> fixture
) : PubliekZoekenScenarioClassFixture<VzerMetActieveErkenningWerdGeschorstScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var erkenning = fixture.Scenario.ErkenningWerdGeregistreerd;
        var erkenningId = erkenning.ErkenningId;
        var actual = fixture.Result.Erkenningen.First(x => x.Key == erkenningId);

        actual.Value.Should().BeEquivalentTo(ErkenningStatus.Geschorst.Value);
    }

    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
