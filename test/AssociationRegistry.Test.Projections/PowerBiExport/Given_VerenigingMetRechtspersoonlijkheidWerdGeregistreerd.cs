namespace AssociationRegistry.Test.Projections.PowerBiExport;

using DecentraalBeheer.Vereniging;
using Scenario.Registratie;
using Vereniging;
using Verenigingstype = Admin.Schema.Detail.Verenigingstype;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
    PowerBiScenarioFixture<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario> fixture)
    : PowerBiScenarioClassFixture<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario>
{
    [Fact]
    public void Sleutel_Is_Niet_Ingevuld()
    {
        fixture.Result.DuplicaatDetectieSleutel.Should().BeEmpty();
    }
}
