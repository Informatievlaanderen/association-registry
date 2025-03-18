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
                  .Verenigingstype.Should().BeEquivalentTo(new VerenigingZoekDocument.Types.Verenigingstype()
                   {
                       Code = Verenigingstype.VZER.Code,
                       Naam = Verenigingstype.VZER.Naam,
                   });

    [Fact]
    public void Verenigingssubtype_Is_NB()
        => fixture.Result
                  .Verenigingssubtype.Should().BeEquivalentTo(new VerenigingZoekDocument.Types.Verenigingssubtype()
                   {
                       Code = Verenigingssubtype.NietBepaald.Code,
                       Naam = Verenigingssubtype.NietBepaald.Naam,
                   });

    /// <summary>
    /// See bug or-2749: properties should not be accidentally reset to default
    /// when updating other properties.
    /// </summary>
    [Fact]
    public void Heeft_Hoofdactiviteiten()
        => fixture.Result.HoofdactiviteitenVerenigingsloket.Should().NotBeNullOrEmpty();
}
