namespace AssociationRegistry.Test.Projections.Beheer.Detail.MaatschappelijkeZetel;

using Scenario.MaatschappelijkeZetelVolgensKbo;

[Collection(nameof(ProjectionContext))]
public class Given_MaatschappelijkeZetelVolgensKBOWerdGewijzigd(
    BeheerDetailScenarioFixture<MaatschappelijkeZetelVolgensKBOWerdGewijzigdScenario> fixture)
    : BeheerDetailScenarioClassFixture<MaatschappelijkeZetelVolgensKBOWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Only_Adres_Is_Updated()
    {
        var maatschappelijkeZetelVolgensKboWerdGewijzigd = fixture.Scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigd;
        var actual = fixture.Result.Locaties.Single(x => x.LocatieId == maatschappelijkeZetelVolgensKboWerdGewijzigd.LocatieId);

        actual.Naam.Should().Be(maatschappelijkeZetelVolgensKboWerdGewijzigd.Naam);
        actual.IsPrimair.Should().Be(maatschappelijkeZetelVolgensKboWerdGewijzigd.IsPrimair);
    }
}
