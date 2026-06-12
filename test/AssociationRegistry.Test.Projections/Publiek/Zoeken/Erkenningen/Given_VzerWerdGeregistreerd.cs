namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using Scenario.Registratie;

[Collection(nameof(ProjectionContext))]
public class Given_VzerWerdGeregistreerd(
    PubliekZoekenScenarioFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario> fixture
) : PubliekZoekenScenarioClassFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario>
{
    [Fact]
    public void Document_Erkenningen_Is_Empty()
    {
        fixture.Result.Erkenningen.Should().BeEmpty();
    }

    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
