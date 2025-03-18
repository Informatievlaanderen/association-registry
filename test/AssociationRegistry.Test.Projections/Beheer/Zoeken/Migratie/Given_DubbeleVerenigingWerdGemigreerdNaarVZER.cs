namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Migratie;

using Scenario.Migratie;

[Collection(nameof(ProjectionContext))]
public class Given_DubbeleVerenigingWerdGemigreerdNaarVZER(BeheerZoekenScenarioFixture<FeitelijkeVerenigingMetBooleanPropertiesWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario> fixture)
    : BeheerZoekenScenarioClassFixture<FeitelijkeVerenigingMetBooleanPropertiesWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
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
