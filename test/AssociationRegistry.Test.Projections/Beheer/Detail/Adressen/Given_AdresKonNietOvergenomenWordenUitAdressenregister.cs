namespace AssociationRegistry.Test.Projections.Beheer.Detail.Adressen;

using Scenario.Adressen;
using System.Linq;

[Collection(nameof(ProjectionContext))]
public class Given_AdresKonNietOvergenomenWordenUitAdressenregister(
    BeheerDetailScenarioFixture<AdresKonNietOvergenomenWordenUitAdressenregisterScenario> fixture)
    : BeheerDetailScenarioClassFixture<AdresKonNietOvergenomenWordenUitAdressenregisterScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void AdresId_And_VerwijstNaar_Is_Null()
    {
        var locatie = fixture.Result.Locaties.Single(x => x.LocatieId == fixture.Scenario.AdresKonNietOvergenomenWordenUitAdressenregister.LocatieId);
        locatie.AdresId.Should().BeNull();
        locatie.VerwijstNaar.Should().BeNull();
        locatie.Adres.Should().NotBeNull();
    }
}
