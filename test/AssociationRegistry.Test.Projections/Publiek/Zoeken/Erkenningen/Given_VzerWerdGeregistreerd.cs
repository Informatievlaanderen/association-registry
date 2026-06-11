namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using Scenario.Registratie;

[Collection(nameof(ProjectionContext))]
public class Given_VzerWerdGeregistreerd(
    PubliekZoekenScenarioFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario> fixture
) : PubliekZoekenScenarioClassFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario>
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
