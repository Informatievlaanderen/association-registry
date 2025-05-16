namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Scenario.Migratie;
using Scenario.Registratie;
using Scenario.Subtypes;
using Vereniging;
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
            Code = AssociationRegistry.Vereniging.Verenigingstype.VZER.Code,
            Naam = AssociationRegistry.Vereniging.Verenigingstype.VZER.Naam,
        });
    }

    [Fact]
    public void Verenigingssubtype_Is_Niet_Bepaald()
    {
        fixture.Result.Verenigingssubtype.Code.Should().BeEquivalentTo(VerenigingssubtypeCode.Default.Code);
        fixture.Result.Verenigingssubtype.Naam.Should().BeEquivalentTo(VerenigingssubtypeCode.Default.Naam);
    }
}
