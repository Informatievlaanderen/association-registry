namespace AssociationRegistry.Test.Projections.Beheer.Detail.Migratie;

using Scenario.Migratie;
using Verenigingstype = Admin.Schema.Detail.Verenigingstype;

[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(BeheerDetailScenarioFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario> fixture)
    : BeheerDetailScenarioClassFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Verenigingstype.Should().Be(new Verenigingstype()
                   {
                       Code = AssociationRegistry.Vereniging.Verenigingstype.VZER.Code,
                       Naam = AssociationRegistry.Vereniging.Verenigingstype.VZER.Naam,
                   });
}
