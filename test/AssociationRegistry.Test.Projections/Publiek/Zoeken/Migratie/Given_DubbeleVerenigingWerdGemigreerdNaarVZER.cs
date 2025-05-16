namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Migratie;

using AssociationRegistry.Test.Projections.Scenario.Dubbels;
using Scenario.Migratie;

[Collection(nameof(ProjectionContext))]
public class Given_DubbeleVerenigingWerdGemigreerdNaarVZER(PubliekZoekenScenarioFixture<FeitelijkeVerenigingMetBooleanPropertiesWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario> fixture)
    : PubliekZoekenScenarioClassFixture<FeitelijkeVerenigingMetBooleanPropertiesWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
{
    [Fact]
    public void IsDubbel()
        => fixture.Result.IsDubbel.Should().BeTrue();

    [Fact]
    public void IsUitgeschreven()
        => fixture.Result.IsUitgeschrevenUitPubliekeDatastroom.Should().BeTrue();

    [Fact]
    public void IsVerwijderd()
        => fixture.Result.IsVerwijderd.Should().BeTrue();
}
