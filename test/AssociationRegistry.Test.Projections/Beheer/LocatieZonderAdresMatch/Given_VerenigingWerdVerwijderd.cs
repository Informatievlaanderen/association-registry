namespace AssociationRegistry.Test.Projections.Beheer.LocatieZonderAdresMatch;

using Scenario.Verwijdering;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdVerwijderd(
    LocatiesZonderAdresMatchScenarioFixture<VerenigingWerdVerwijderdScenario> fixture
) : LocatiesZonderAdresMatchScenarioClassFixture<VerenigingWerdVerwijderdScenario>
{
    [Fact]
    public void Then_No_Document()
    {
        fixture.Result.Should().BeEmpty();
    }
}
