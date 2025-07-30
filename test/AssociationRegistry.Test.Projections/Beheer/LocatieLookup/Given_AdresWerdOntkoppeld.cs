namespace AssociationRegistry.Test.Projections.Beheer.LocatieLookup;

using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class AdresWerdOntkoppeldVanAdressenregister(
    LocatieLookupScenarioFixture<AdresWerdOntkoppeldVanAdressenregisterScenario> fixture)
    : LocatieLookupScenarioClassFixture<AdresWerdOntkoppeldVanAdressenregisterScenario>
{
    [Fact]
    public void Adres_Properties_Are_Correctly_Set()
    {
        fixture
                    .Result
                    .SingleOrDefault(x => x.Id ==
                                          $"{fixture.Scenario.VCode}-{fixture.Scenario.AdresWerdOntkoppeldVanAdressenregister.LocatieId}")
                    .Should()
                    .BeNull();
    }
}
