namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Locaties.Kbo;

using Formats;
using AssociationRegistry.Test.Projections.Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_MaatschappelijkeZetelWerdGewijzigdInKbo(
    DuplicateDetectionScenarioFixture<MaatschappelijkeZetelWerdGewijzigdInKboScenario> fixture)
    : DuplicateDetectionClassFixture<MaatschappelijkeZetelWerdGewijzigdInKboScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var locatie = fixture.Scenario.MaatschappelijkeZetelWerdGewijzigdInKboFromFirstLocation;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == locatie.Locatie.LocatieId);

        actual.Locatietype.Should().BeEquivalentTo(locatie.Locatie.Locatietype);
        actual.Naam.Should().BeEquivalentTo(locatie.Locatie.Naam);
        actual.IsPrimair.Should().Be(locatie.Locatie.IsPrimair);
        actual.Gemeente.Should().BeEquivalentTo(locatie.Locatie.Adres.Gemeente);
        actual.Postcode.Should().BeEquivalentTo(locatie.Locatie.Adres.Postcode);
        actual.Adresvoorstelling.Should().BeEquivalentTo(locatie.Locatie.Adres.ToAdresString());
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Locaties.Select(x => x.LocatieId).Should().BeInAscendingOrder();
    }
}
