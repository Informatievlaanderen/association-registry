namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Locaties.Kbo;

using Formats;
using AssociationRegistry.Test.Projections.Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_MaatschappelijkeZetelWerdOvergenomenUitKbo(
    DuplicateDetectionScenarioFixture<MaatschappelijkeZetelWerdOvergenomenUitKboScenario> fixture)
    : DuplicateDetectionClassFixture<MaatschappelijkeZetelWerdOvergenomenUitKboScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        var maatschappelijkeZetel = fixture.Scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == maatschappelijkeZetel.Locatie.LocatieId);
        var vCode = fixture.Scenario.AggregateId;

        actual.Locatietype.Should().BeEquivalentTo(maatschappelijkeZetel.Locatie.Locatietype);
        actual.Naam.Should().BeEquivalentTo(maatschappelijkeZetel.Locatie.Naam);
        actual.IsPrimair.Should().Be(maatschappelijkeZetel.Locatie.IsPrimair);
        actual.Gemeente.Should().BeEquivalentTo(maatschappelijkeZetel.Locatie.Adres.Gemeente);
        actual.Postcode.Should().BeEquivalentTo(maatschappelijkeZetel.Locatie.Adres.Postcode);
        actual.Adresvoorstelling.Should().BeEquivalentTo(maatschappelijkeZetel.Locatie.Adres.ToAdresString());
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Locaties.Select(x => x.LocatieId).Should().BeInAscendingOrder();
    }
}
