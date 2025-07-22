namespace AssociationRegistry.Test.Projections.Beheer.LocatieLookup;

using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresWerdOpnieuwOvergenomenUitAdressenregister(
    LocatieLookupScenarioFixture<AdresWerdOpnieuwOvergenomenUitAdressenregisterScenario> fixture)
    : LocatieLookupScenarioClassFixture<AdresWerdOpnieuwOvergenomenUitAdressenregisterScenario>
{
    [Fact]
    public void Adres_Properties_Are_Correctly_Set()
    {
        var adresWerdOpnieuwOvergenomenUitAdressenregister = fixture.Scenario.AdresWerdOpnieuwOvergenomenUitAdressenregister;

        var result = fixture.Result.Single(x => x.LocatieId == adresWerdOpnieuwOvergenomenUitAdressenregister.LocatieId);

        result.AdresId.Should().Be(adresWerdOpnieuwOvergenomenUitAdressenregister.AdresId.ToId());
        result.AdresPuri.Should().Be(adresWerdOpnieuwOvergenomenUitAdressenregister.AdresId.Bronwaarde);
    }
}
