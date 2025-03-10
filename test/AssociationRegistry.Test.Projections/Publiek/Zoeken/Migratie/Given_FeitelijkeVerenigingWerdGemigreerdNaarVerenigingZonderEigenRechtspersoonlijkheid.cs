namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Migratie;

using Public.Schema.Search;
using Scenario.Migratie;
using Vereniging;


[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(PubliekZoekenScenarioFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario> fixture)
    : PubliekZoekenScenarioClassFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Verenigingstype.Should().BeEquivalentTo(new VerenigingZoekDocument.Types.VerenigingsType()
                   {
                       Code = Verenigingstype.VZER.Code,
                       Naam = Verenigingstype.VZER.Naam,
                   });

    // [Fact]
    // public void Verenigingssubtype_Is_NNB()
    //     => fixture.Result
    //               .Verenigingssubtype.Should().BeEquivalentTo(new PubliekVerenigingDetailDocument.Types.Verenigingssubtype()
    //                {
    //                    Code = Verenigingssubtype.NogNietBepaald.Code,
    //                    Naam = Verenigingssubtype.NogNietBepaald.Naam,
    //                });
}
