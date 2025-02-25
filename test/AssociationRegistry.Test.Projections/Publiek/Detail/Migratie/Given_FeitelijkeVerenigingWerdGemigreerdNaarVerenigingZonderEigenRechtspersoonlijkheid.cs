namespace AssociationRegistry.Test.Projections.Publiek.Detail.Migratie;

using Public.Schema.Detail;

[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(PubliekDetailScenarioFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario> fixture)
    : PubliekDetailScenarioClassFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Verenigingstype.Should().BeEquivalentTo(new PubliekVerenigingDetailDocument.VerenigingsType()
                   {
                       Code = Vereniging.Verenigingstype.VZER.Code,
                       Naam = Vereniging.Verenigingstype.VZER.Naam,
                   });
}
