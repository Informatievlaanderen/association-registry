namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Migratie;

using Admin.Schema.Search;
using Scenario.Migratie;

[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(BeheerZoekenScenarioFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario> fixture)
    : BeheerZoekenScenarioClassFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
{

    [Fact]
    public void VerenigingsType_Is_Vzer()
        => fixture.Result
                  .Verenigingstype.Should().BeEquivalentTo(new VerenigingZoekDocument.VerenigingsType
                   {
                       Code = Vereniging.Verenigingstype.VZER.Code,
                       Naam = Vereniging.Verenigingstype.VZER.Naam,
                   });
}
