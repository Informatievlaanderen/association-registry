namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Locaties.Kbo;

using Scenario.Locaties;

[Collection(nameof(ProjectionContext))]
public class Given_MaatschappelijkeZetelWerdVerwijderdUitKbo(
    BeheerZoekenScenarioFixture<MaatschappelijkeZetelWerdVerwijderdUitKboScenario> fixture)
    : BeheerZoekenScenarioClassFixture<MaatschappelijkeZetelWerdVerwijderdUitKboScenario>
{
    [Fact]
    public void Document_Maatschappelijke_Zetel_Werd_Verwijderd_()
    {
        fixture.Result.Locaties.Should()
               .NotContain(x => x.LocatieId == fixture.Scenario.MaatschappelijkeZetelWerdVerwijderdUitKboFirstLocation
                                                      .Locatie.LocatieId);
    }

    [Fact]
    public void Order_Is_Correct()
    {
        fixture.Result.Locaties.Select(x => x.LocatieId).Should().BeInAscendingOrder();
    }
}
