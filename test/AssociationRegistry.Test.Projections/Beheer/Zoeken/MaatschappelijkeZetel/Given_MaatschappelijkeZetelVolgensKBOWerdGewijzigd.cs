namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.MaatschappelijkeZetel;

using Scenario.MaatschappelijkeZetelVolgensKbo;

[Collection(nameof(ProjectionContext))]
public class Given_MaatschappelijkeZetelVolgensKBOWerdGewijzigd(
    BeheerZoekenScenarioFixture<MaatschappelijkeZetelVolgensKBOWerdGewijzigdScenario> fixture)
    : BeheerZoekenScenarioClassFixture<MaatschappelijkeZetelVolgensKBOWerdGewijzigdScenario>
{
    [Fact]
    public void Only_Adres_Is_Updated()
    {
        var maatschappelijkeZetelVolgensKboWerdGewijzigd = fixture.Scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigd;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == maatschappelijkeZetelVolgensKboWerdGewijzigd.LocatieId);

        actual.Naam.Should().Be(maatschappelijkeZetelVolgensKboWerdGewijzigd.Naam);
        actual.IsPrimair.Should().Be(maatschappelijkeZetelVolgensKboWerdGewijzigd.IsPrimair);
    }
}
