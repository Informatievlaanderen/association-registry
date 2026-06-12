namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.NaamWerdGewijzigd.Kbo;

using Scenario.NaamWerdGewijzigd.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_NaamWerdGewijzigdInKbo(
    BeheerZoekenScenarioFixture<NaamWerdGewijzigdInKboScenario> fixture)
    : BeheerZoekenScenarioClassFixture<NaamWerdGewijzigdInKboScenario>
{
    [Fact]
    public void Document_Naam_Werd_Gewijzigd()
    {
        fixture.Result.Naam.Should()
               .BeEquivalentTo(fixture.Scenario.NaamWerdGewijzigdInKbo.Naam);
    }
}
