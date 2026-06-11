namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Erkenningen;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;
using Publiek.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ActieveErkenningWerdVerwijderdMetBehoudenVerlopenErkenning(
    BeheerZoekenScenarioFixture<VzerMetActieveTeVerwijderenErkenningEnBehoudenVerlopenErkenningScenario> fixture
) : BeheerZoekenScenarioClassFixture<VzerMetActieveTeVerwijderenErkenningEnBehoudenVerlopenErkenningScenario>
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
