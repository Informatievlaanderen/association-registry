namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_VerlopenErkenningWerdGewijzigdNaarActieveErkenning(
    PubliekZoekenScenarioFixture<VzerMetVerlopenErkenningenGewijzigdNaarEnkeleActieveErkenningScenario> fixture
) : PubliekZoekenScenarioClassFixture<VzerMetVerlopenErkenningenGewijzigdNaarEnkeleActieveErkenningScenario>
{
    [Fact]
    public void Document_Erkenningen_Have_Expected_Statuses()
    {
        fixture
            .Result.Erkenningen.Should()
            .BeEquivalentTo(
                new Dictionary<int, string>()
                {
                    {
                        fixture.Scenario.VerlopenErkenningWerdGerigstreerd.ErkenningId,
                        fixture.Scenario.VerlopenErkenningWerdGerigstreerd.Status
                    },
                    {
                        fixture.Scenario.TeActiverenVerlopenErkenningWerdGeregistreerd.ErkenningId,
                        ErkenningStatus.Actief.Value
                    },
                }
            );
    }

    [Fact]
    public void IsErkend_Is_True()
    {
        fixture.Result.IsErkend.Should().BeTrue();
    }
}
