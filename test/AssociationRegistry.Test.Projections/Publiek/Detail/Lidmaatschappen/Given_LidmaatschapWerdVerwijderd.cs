namespace AssociationRegistry.Test.Projections.Publiek.Detail.Lidmaatschappen;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdVerwijderd(PubliekDetailScenarioFixture<LidmaatschapWerdVerwijderdScenario> fixture)
    : PubliekDetailScenarioClassFixture<LidmaatschapWerdVerwijderdScenario>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Lidmaatschappen.Should().BeEmpty();
}
