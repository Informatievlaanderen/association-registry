namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Erkenningen;

using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_NietActieveErkenningWerdGeregistreerd(
    BeheerZoekenScenarioFixture<VzerMetErkenningInAanvraagWerdGeregistreerdScenario> fixture
) : BeheerZoekenScenarioClassFixture<VzerMetErkenningInAanvraagWerdGeregistreerdScenario>
{
    [Fact]
    public void Document_Erkenningen_Has_Geregistreerde_Status()
    {
        fixture
           .Result.Erkenningen.Should()
           .BeEquivalentTo(
                new Dictionary<int, string>()
                {
                    {
                        fixture.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                        fixture.Scenario.ErkenningWerdGeregistreerd.Status
                    },
                }
            );
    }

    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
