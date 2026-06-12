namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ActieveErkenningWerdVerwijderdEnActieveWerdBehouden(
    PubliekZoekenScenarioFixture<VzerMetActieveErkenningenWerdVerwijderdEnActieveBehoudenScenario> fixture
) : PubliekZoekenScenarioClassFixture<VzerMetActieveErkenningenWerdVerwijderdEnActieveBehoudenScenario>
{
    [Fact]
    public void Document_Erkenningen_Contains_ActieveErkenning()
    {
        fixture
            .Result.Erkenningen.Should()
            .BeEquivalentTo(
                new Dictionary<int, string>()
                {
                    { fixture.Scenario.ActieveErkenningWerdGeregistreerd.ErkenningId, ErkenningStatus.Actief.Value },
                }
            );
    }

    [Fact]
    public void IsErkend_Is_True()
    {
        fixture.Result.IsErkend.Should().BeTrue();
    }
}
