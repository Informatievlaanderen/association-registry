namespace AssociationRegistry.Test.Projections.Beheer.Detail.Doelgroepen;

using Scenario.Doelgroepen;

[Collection(nameof(ProjectionContext))]
public class Given_DoelgroepWerdGewijzigdd(
    BeheerDetailScenarioFixture<DoelgroepWerdGewijzigdScenario> fixture)
    : BeheerDetailScenarioClassFixture<DoelgroepWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.Doelgroep.Should().BeEquivalentTo(fixture.Scenario.DoelgroepWerdGewijzigd.Doelgroep);
    }
}
