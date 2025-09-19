namespace AssociationRegistry.Test.Projections.PowerBiExport;

using DecentraalBeheer.Vereniging;
using Scenario.Registratie;
using Vereniging;
using Verenigingstype = Admin.Schema.Detail.Verenigingstype;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdNaDubbels(
    PowerBiScenarioFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdNaDubbelsScenario> fixture)
    : PowerBiScenarioClassFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdNaDubbelsScenario>
{
    [Fact]
    public void Sleutel_Is_Ingevuld()
    {
        fixture.Result.DuplicaatDetectieSleutel.Should().Be(
            fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                   .DuplicaatDetectieInfo.BevestigingstokenSleutel);
    }
}
