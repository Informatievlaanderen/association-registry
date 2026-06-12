namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Erkenningen;

using Scenario.Registratie;

[Collection(nameof(ProjectionContext))]
public class Given_VzerWerdGeregistreerd(
    BeheerZoekenScenarioFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario> fixture
) : BeheerZoekenScenarioClassFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario>
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
