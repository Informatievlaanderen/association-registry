namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ActieveErkenningWerdVerwijderdMetBehoudenVerlopenErkenning(
    PubliekZoekenScenarioFixture<VzerMetActieveTeVerwijderenErkenningEnBehoudenVerlopenErkenningScenario> fixture
) : PubliekZoekenScenarioClassFixture<VzerMetActieveTeVerwijderenErkenningEnBehoudenVerlopenErkenningScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        fixture
            .Result.Erkenningen.Should()
            .BeEquivalentTo(
                new Dictionary<int, string>()
                {
                    { fixture.Scenario.VerlopenErkenningWerdGerigstreerd.ErkenningId, ErkenningStatus.Verlopen.Value },
                }
            );
    }

    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
