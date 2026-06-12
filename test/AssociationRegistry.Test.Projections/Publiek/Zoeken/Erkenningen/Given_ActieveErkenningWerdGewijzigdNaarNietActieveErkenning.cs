namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ActieveErkenningWerdGewijzigdNaarNietActieveErkenning(
    PubliekZoekenScenarioFixture<VzerMetActieveErkenningWerdGewijzigdNaarVerlopenErkenning> fixture
) : PubliekZoekenScenarioClassFixture<VzerMetActieveErkenningWerdGewijzigdNaarVerlopenErkenning>
{
    [Fact]
    public void Document_Erkenningen_Are_Verlopen()
    {
        fixture
            .Result.Erkenningen.Should()
            .BeEquivalentTo(
                new Dictionary<int, string>()
                {
                    { fixture.Scenario.VerlopenErkenningWerdGerigstreerd.ErkenningId, ErkenningStatus.Verlopen.Value },
                    { fixture.Scenario.ActieveErkenningWerdGeregistreerd.ErkenningId, ErkenningStatus.Verlopen.Value },
                }
            );
    }

    [Fact]
    public void IsErkend_Is_True()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
