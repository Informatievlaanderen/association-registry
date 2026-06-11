namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ActieveErkenningWerdVerwijderdMetVerlopenErkenning(
    PubliekZoekenScenarioFixture<VerenigingMetActieveEnVerlopenErkenningActieveErkenningWerdVerwijderdScenario> fixture
) : PubliekZoekenScenarioClassFixture<VerenigingMetActieveEnVerlopenErkenningActieveErkenningWerdVerwijderdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var erkenning = fixture.Scenario.TeVerwijderenActieveErkenningWerdVerwijderd;
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
                }
            );
    }

    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
