namespace AssociationRegistry.Test.Projections.PowerBiExport.Registratie;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.Projections.Scenario.Registratie;
using Verenigingstype = Admin.Schema.Detail.Verenigingstype;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
    PowerBiScenarioFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario> fixture)
    : PowerBiScenarioClassFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario>
{
    [Fact]
    public void Verenigingstype_Is_VZER()
    {
        fixture.Result.Verenigingstype.Should().BeEquivalentTo(new Verenigingstype()
        {
            Code = DecentraalBeheer.Vereniging.Verenigingstype.VZER.Code,
            Naam = DecentraalBeheer.Vereniging.Verenigingstype.VZER.Naam,
        });
    }

    [Fact]
    public void Verenigingssubtype_Is_Niet_Bepaald()
    {
        fixture.Result.Verenigingssubtype.Code.Should().BeEquivalentTo(VerenigingssubtypeCode.Default.Code);
        fixture.Result.Verenigingssubtype.Naam.Should().BeEquivalentTo(VerenigingssubtypeCode.Default.Naam);
    }
}
