namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Adressen;

using Formats;
using Scenario.Adressen;

[Collection(nameof(ProjectionContext))]
public class Given_AdresWerdGewijzigdInAdressenregister(BeheerZoekenScenarioFixture<AdresWerdGewijzigdInAdressenregisterScenario> fixture)
    : BeheerZoekenScenarioClassFixture<AdresWerdGewijzigdInAdressenregisterScenario>
{
    [Fact]
    public void Adres_Is_Updated()
    {
        var locatie  = fixture.Result.Locaties.Single(x => x.LocatieId == fixture.Scenario.AdresWerdGewijzigdInAdressenregister.LocatieId);
        locatie.Adresvoorstelling.Should().Be(fixture.Scenario.AdresWerdGewijzigdInAdressenregister.Adres.ToAdresString());
        locatie.Postcode.Should().Be(fixture.Scenario.AdresWerdGewijzigdInAdressenregister.Adres.Postcode);
        locatie.Gemeente.Should().Be(fixture.Scenario.AdresWerdGewijzigdInAdressenregister.Adres.Gemeente);
    }
}
