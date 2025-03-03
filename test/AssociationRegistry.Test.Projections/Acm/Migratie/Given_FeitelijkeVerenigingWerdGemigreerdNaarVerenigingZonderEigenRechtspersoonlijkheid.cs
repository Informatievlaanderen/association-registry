namespace AssociationRegistry.Test.Projections.Acm.Migratie;

using AssociationRegistry.Acm.Schema.VerenigingenPerInsz;
using Scenario.Migratie;

[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(
    VerenigingenPerInszScenarioFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario> fixture)
    : VerenigingenPerInszScenarioClassFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
{
    [Fact]
    public void Then_VerenigingsType_Is_Vzer()
    {
        var vzer =
            fixture.Result.Verenigingen.Single(x => x.VCode == fixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode);

        vzer.Verenigingstype.Should().BeEquivalentTo(new Verenigingstype(AssociationRegistry.Vereniging.Verenigingstype.Verenigingstype.VZER.Code, AssociationRegistry.Vereniging.Verenigingstype.Verenigingstype.VZER.Naam));
    }
}
