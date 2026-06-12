namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ActieveErkenningWerdVerwijderdEnActieveWerdBehouden(
    BeheerZoekenScenarioFixture<VzerMetActieveErkenningenWerdVerwijderdEnActieveBehoudenScenario> fixture
) : BeheerZoekenScenarioClassFixture<VzerMetActieveErkenningenWerdVerwijderdEnActieveBehoudenScenario>
{
    [Fact]
    public void Document_Erkenningen_Has_One_Actieve_Erkenning()
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
