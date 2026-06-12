namespace AssociationRegistry.Test.Projections.Beheer.Detail.NaamWerdGewijzigd.Vzer;

using Scenario.NaamWerdGewijzigd.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_NaamWerdGewijzigd(
    BeheerDetailScenarioFixture<NaamWerdGewijzigdScenario> fixture)
    : BeheerDetailScenarioClassFixture<NaamWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Has_Naam_Gewijzigd()
    {
        fixture.Result.Naam.Should()
               .BeEquivalentTo(fixture.Scenario.NaamWerdGewijzigd.Naam);
    }
}
