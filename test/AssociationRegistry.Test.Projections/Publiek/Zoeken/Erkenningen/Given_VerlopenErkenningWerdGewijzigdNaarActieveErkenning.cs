namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_VerlopenErkenningWerdGewijzigdNaarActieveErkenning(
    PubliekZoekenScenarioFixture<VerenigingMetVerlopenErkenningenGewijzigdNaarEnkeleActieveErkenningScenario> fixture
) : PubliekZoekenScenarioClassFixture<VerenigingMetVerlopenErkenningenGewijzigdNaarEnkeleActieveErkenningScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var erkenning = fixture.Scenario.TeActiverenVerlopenErkenningErkenningWerdGeactiveerd;
        var erkenningId = erkenning.ErkenningId;
        var actual = fixture.Result.Erkenningen.Where(x => x.Key == erkenningId).ToList();

        actual.Should().NotContainKey(erkenningId);
        actual
            .Should()
            .BeEquivalentTo(
                new Dictionary<int, string>
                {
                    [fixture.Scenario.VerlopenErkenningWerdGerigstreerd.ErkenningId] = fixture
                        .Scenario
                        .VerlopenErkenningWerdGerigstreerd
                        .Status,
                    [fixture.Scenario.TeActiverenVerlopenErkenningErkenningWerdGeregistreerd.ErkenningId] =
                        ErkenningStatus.Actief.Value,
                }
            );
    }

    [Fact]
    public void IsErkend_Is_True()
    {
        fixture.Result.IsErkend.Should().BeTrue();
    }
}
