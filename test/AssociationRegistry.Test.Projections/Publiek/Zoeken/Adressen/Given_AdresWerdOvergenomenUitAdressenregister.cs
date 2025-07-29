namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Adressen;

using Formats;
using AssociationRegistry.Test.Projections.Scenario.Adressen;
using Beheer.DuplicateDetection;

[Collection(nameof(ProjectionContext))]
public class Given_AdresWerdOvergenomenUitAdressenregister(DuplicateDetectionScenarioFixture<AdresWerdOvergenomenUitAdressenregisterScenario> fixture)
    : DuplicateDetectionClassFixture<AdresWerdOvergenomenUitAdressenregisterScenario>
{
    [Fact]
    public void Adres_Is_Updated()
    {
        var locatie  = fixture.Result.Locaties.Single(x => x.LocatieId == fixture.Scenario.AdresWerdOvergenomenUitAdressenregister.LocatieId);
        locatie.Adresvoorstelling.Should().Be(fixture.Scenario.AdresWerdOvergenomenUitAdressenregister.Adres.ToAdresString());
        locatie.Postcode.Should().Be(fixture.Scenario.AdresWerdOvergenomenUitAdressenregister.Adres.Postcode);
        locatie.Gemeente.Should().Be(fixture.Scenario.AdresWerdOvergenomenUitAdressenregister.Adres.Gemeente);
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Locaties.Select(x => x.LocatieId).Should().BeInAscendingOrder();
    }
}
