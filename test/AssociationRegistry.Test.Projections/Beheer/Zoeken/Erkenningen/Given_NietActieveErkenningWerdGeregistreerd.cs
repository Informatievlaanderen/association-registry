namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Erkenningen;

using AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;
using Publiek.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_NietActieveErkenningWerdGeregistreerd(
    BeheerZoekenScenarioFixture<VzerMetErkenningInAanvraagWerdGeregistreerdScenario> fixture
) : BeheerZoekenScenarioClassFixture<VzerMetErkenningInAanvraagWerdGeregistreerdScenario>
{
    [Fact]
    public void Document_Is_Updated()
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
