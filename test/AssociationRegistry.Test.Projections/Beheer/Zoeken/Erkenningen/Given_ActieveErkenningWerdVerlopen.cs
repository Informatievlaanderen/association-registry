namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Erkenningen;

using DecentraalBeheer.Vereniging.Erkenningen;
using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ActieveErkenningWerdVerlopen(
    BeheerZoekenScenarioFixture<VzerMetActieveErkenningWerdVerlopenScenario> fixture
) : BeheerZoekenScenarioClassFixture<VzerMetActieveErkenningWerdVerlopenScenario>
{
    [Fact]
    public void Document_Erkenningen_Has_One_Verlopen_Erkenning()
    {
        fixture
           .Result.Erkenningen.Should()
           .BeEquivalentTo(
                new Dictionary<int, string>()
                {
                    { fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId, ErkenningStatus.Verlopen.Value },
                }
            );
    }

    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
