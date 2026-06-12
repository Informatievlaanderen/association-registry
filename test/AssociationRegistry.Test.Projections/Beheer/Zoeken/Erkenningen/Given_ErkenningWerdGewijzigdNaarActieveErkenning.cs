namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdGewijzigdNaarActieveErkenning(
    BeheerZoekenScenarioFixture<VzerMetGeregistreerdeErkenningWordtGewijzigdNaarActiefScenario> fixture
) : BeheerZoekenScenarioClassFixture<VzerMetGeregistreerdeErkenningWordtGewijzigdNaarActiefScenario>
{
    [Fact]
    public void Document_Erkenningen_Contains_ActieveErkenning()
    {
        fixture
           .Result.Erkenningen.Should()
           .BeEquivalentTo(
                new Dictionary<int, string>()
                {
                    { fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId, ErkenningStatus.Actief.Value },
                }
            );
    }

    [Fact]
    public void IsErkend_Is_True()
    {
        fixture.Result.IsErkend.Should().BeTrue();
    }
}
