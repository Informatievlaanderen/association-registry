namespace AssociationRegistry.Test.Projections.Beheer.Detail.KorteNaam.Vzer;

using AssociationRegistry.Test.Projections.Scenario.KorteNaamWerdGewijzigd.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_KorteNaamWerdGewijzigd(
    BeheerDetailScenarioFixture<KorteNaamWerdGewijzigdScenario> fixture)
    : BeheerDetailScenarioClassFixture<KorteNaamWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.KorteNaam.Should()
               .BeEquivalentTo(fixture.Scenario.KorteNaamWerdGewijzigd.KorteNaam);
    }
}
