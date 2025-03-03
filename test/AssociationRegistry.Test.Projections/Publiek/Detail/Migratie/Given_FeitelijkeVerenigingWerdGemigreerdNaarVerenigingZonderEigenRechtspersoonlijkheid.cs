namespace AssociationRegistry.Test.Projections.Publiek.Detail.Migratie;

using Public.Schema.Detail;
using Scenario.Migratie;
using Vereniging.Verenigingstype;

[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(PubliekDetailScenarioFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario> fixture)
    : PubliekDetailScenarioClassFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Verenigingstype.Should().BeEquivalentTo(new PubliekVerenigingDetailDocument.VerenigingsType()
                   {
                       Code = Verenigingstype.VZER.Code,
                       Naam = Verenigingstype.VZER.Naam,
                   });
}
