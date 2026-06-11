namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Erkenningen;

using AssociationRegistry.Test.Projections.Scenario.Registratie;
using Publiek.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_VzerWerdGeregistreerd(
    BeheerZoekenScenarioFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario> fixture
) : BeheerZoekenScenarioClassFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.Erkenningen.Should().BeEmpty();
    }

    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
