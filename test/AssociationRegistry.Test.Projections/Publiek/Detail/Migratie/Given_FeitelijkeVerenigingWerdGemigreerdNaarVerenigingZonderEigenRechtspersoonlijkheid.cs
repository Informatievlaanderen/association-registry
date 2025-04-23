namespace AssociationRegistry.Test.Projections.Publiek.Detail.Migratie;

using Public.Schema.Detail;
using Scenario.Migratie;
using Vereniging;


[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(PubliekDetailScenarioFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario> fixture)
    : PubliekDetailScenarioClassFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
{
    [Fact]
    public void Verenigingstype_Is_VZER()
        => fixture.Result
                  .Verenigingstype.Should().BeEquivalentTo(new PubliekVerenigingDetailDocument.Types.Verenigingstype()
                   {
                       Code = Verenigingstype.VZER.Code,
                       Naam = Verenigingstype.VZER.Naam,
                   });

    [Fact]
    public void Verenigingssubtype_Is_NB()
        => fixture.Result
                  .Verenigingssubtype.Should().BeEquivalentTo(new PubliekVerenigingDetailDocument.Types.Verenigingssubtype()
                   {
                       Code = string.Empty,
                       Naam = string.Empty,
                   });
}
