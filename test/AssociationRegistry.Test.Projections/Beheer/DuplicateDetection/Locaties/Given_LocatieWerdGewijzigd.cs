namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Locaties;

using Formats;
using AssociationRegistry.Test.Projections.Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdGewijzigd(
    DuplicateDetectionScenarioFixture<LocatieWerdGewijzigdScenario> fixture)
    : DuplicateDetectionClassFixture<LocatieWerdGewijzigdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var locatieWerdGewijzigd = fixture.Scenario.LocatieWerdGewijzigd;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == locatieWerdGewijzigd.Locatie.LocatieId);
        var vCode = fixture.Scenario.AggregateId;

        actual.Locatietype.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Locatietype);
        actual.Naam.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Naam);
        actual.IsPrimair.Should().Be(locatieWerdGewijzigd.Locatie.IsPrimair);
        actual.Gemeente.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres.Gemeente);
        actual.Postcode.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres.Postcode);
        actual.Adresvoorstelling.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres.ToAdresString());
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Locaties.Select(x => x.LocatieId).Should().BeInAscendingOrder();
    }
}
