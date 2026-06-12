namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection.Locaties.Kbo;

using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_MaatschappelijkeZetelVolgensKBOWerdGewijzigd(
    DuplicateDetectionScenarioFixture<MaatschappelijkeZetelVolgensKBOWerdGewijzigdScenario> fixture)
    : DuplicateDetectionClassFixture<MaatschappelijkeZetelVolgensKBOWerdGewijzigdScenario>
{
    [Fact]
    public void Document_Maatschappelijke_Zetel_Volgens_Kbo_Werd_Gewijzigd()
    {
        var locatie = fixture.Scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigdFirstLocation;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == locatie.LocatieId);

        actual.Naam.Should().BeEquivalentTo(locatie.Naam);
        actual.IsPrimair.Should().Be(locatie.IsPrimair);
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Locaties.Select(x => x.LocatieId).Should().BeInAscendingOrder();
    }
}
