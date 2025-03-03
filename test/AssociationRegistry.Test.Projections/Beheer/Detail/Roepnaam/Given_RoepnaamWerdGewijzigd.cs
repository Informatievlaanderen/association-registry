namespace AssociationRegistry.Test.Projections.Beheer.Detail.Roepnaam;

using AssociationRegistry.Test.Projections.Scenario.Roepnaam;

[Collection(nameof(ProjectionContext))]
public class Given_RoepnaamWerdGewijzigd(
    BeheerDetailScenarioFixture<RoepnaamWerdGewijzigdScenario> fixture)
    : BeheerDetailScenarioClassFixture<RoepnaamWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.Roepnaam.Should()
               .BeEquivalentTo(fixture.Scenario.RoepnaamWerdGewijzigd.Roepnaam);
    }
}
