namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Locaties;

using Formats;
using AssociationRegistry.Test.Projections.Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_LocatieWerdToegevoegd(
    DuplicateDetectionScenarioFixture<LocatieWerdToegevoegdScenario> fixture)
    : DuplicateDetectionClassFixture<LocatieWerdToegevoegdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var locatieWerdToegevoegd = fixture.Scenario.LocatieWerdToegevoegd;
        var vCode = fixture.Scenario.VCode;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == locatieWerdToegevoegd.Locatie.LocatieId);

        actual.Locatietype.Should().BeEquivalentTo(locatieWerdToegevoegd.Locatie.Locatietype);
        actual.Naam.Should().BeEquivalentTo(locatieWerdToegevoegd.Locatie.Naam);
        actual.IsPrimair.Should().Be(locatieWerdToegevoegd.Locatie.IsPrimair);
        actual.Gemeente.Should().BeEquivalentTo(locatieWerdToegevoegd.Locatie.Adres.Gemeente);
        actual.Postcode.Should().BeEquivalentTo(locatieWerdToegevoegd.Locatie.Adres.Postcode);
        actual.Adresvoorstelling.Should().BeEquivalentTo(locatieWerdToegevoegd.Locatie.Adres.ToAdresString());
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Locaties.Select(x => x.LocatieId).Should().BeInAscendingOrder();
    }
}
