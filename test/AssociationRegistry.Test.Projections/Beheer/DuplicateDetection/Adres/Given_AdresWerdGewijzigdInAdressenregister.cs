namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Adres;

using Formats;
using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresWerdGewijzigdInAdressenregister(DuplicateDetectionScenarioFixture<AdresWerdGewijzigdInAdressenregisterScenario> fixture)
    : DuplicateDetectionClassFixture<AdresWerdGewijzigdInAdressenregisterScenario>
{
    [Fact]
    public void Adres_Is_Updated()
    {
        var locatie  = fixture.Result.Locaties.Single(x => x.LocatieId == fixture.Scenario.AdresWerdGewijzigdInAdressenregister.LocatieId);
        locatie.Adresvoorstelling.Should().Be(fixture.Scenario.AdresWerdGewijzigdInAdressenregister.Adres.ToAdresString());
        locatie.Postcode.Should().Be(fixture.Scenario.AdresWerdGewijzigdInAdressenregister.Adres.Postcode);
        locatie.Gemeente.Should().Be(fixture.Scenario.AdresWerdGewijzigdInAdressenregister.Adres.Gemeente);
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Locaties.Select(x => x.LocatieId).Should().BeInAscendingOrder();
    }
}
