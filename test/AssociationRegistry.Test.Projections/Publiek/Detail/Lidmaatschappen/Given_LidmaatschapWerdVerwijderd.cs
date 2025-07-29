namespace AssociationRegistry.Test.Projections.Publiek.Detail.Lidmaatschappen;

using Scenario.Lidmaatschappen;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdVerwijderd(PubliekDetailScenarioFixture<LidmaatschapWerdVerwijderdScenario> fixture)
    : PubliekDetailScenarioClassFixture<LidmaatschapWerdVerwijderdScenario>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Lidmaatschappen.Should()
                  .NotContain(x => x.LidmaatschapId == fixture.Scenario.LidmaatschapWerdVerwijderd.Lidmaatschap.LidmaatschapId);
}
