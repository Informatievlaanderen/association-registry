namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.Detail;
using Scenario.Migratie;
using Scenario.Registratie;
using Scenario.Subtypes;
using Verenigingssubtype = Vereniging.Verenigingssubtype;

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
            Code = Vereniging.Verenigingstype.VZER.Code,
            Naam = Vereniging.Verenigingstype.VZER.Naam,
        });
    }

    [Fact]
    public void Verenigingssubtype_Is_Niet_Bepaald()
    {
        fixture.Result.Verenigingssubtype.Should().BeEquivalentTo(Verenigingssubtype.Default);
    }
}
