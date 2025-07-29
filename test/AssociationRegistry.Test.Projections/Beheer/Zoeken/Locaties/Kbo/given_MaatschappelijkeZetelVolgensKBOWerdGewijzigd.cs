namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Locaties.Kbo;

using AssociationRegistry.Test.Projections.Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_MaatschappelijkeZetelVolgensKBOWerdGewijzigd(
    BeheerZoekenScenarioFixture<MaatschappelijkeZetelVolgensKBOWerdGewijzigdScenario> fixture)
    : BeheerZoekenScenarioClassFixture<MaatschappelijkeZetelVolgensKBOWerdGewijzigdScenario>
{
    [Fact]
    public void Document_Is_Updated()
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
