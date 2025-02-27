namespace AssociationRegistry.Test.Projections.Beheer.Detail.KorteNaam.Kbo;

using Scenario.KorteNaamWerdGewijzigd.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_KorteNaamWerdGewijzigdInKbo(
    BeheerDetailScenarioFixture<KorteNaamWerdGewijzigdInKboScenario> fixture)
    : BeheerDetailScenarioClassFixture<KorteNaamWerdGewijzigdInKboScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.KorteNaam.Should()
               .BeEquivalentTo(fixture.Scenario.KorteNaamWerdGewijzigdInKbo.KorteNaam);
    }
}
