namespace AssociationRegistry.Test.Projections.Beheer.Detail.KorteBeschijving;

using AssociationRegistry.Test.Projections.Scenario.KorteBeschijving;

[Collection(nameof(ProjectionContext))]
public class Given_KorteBeschrijvingWerdGewijzigd(
    BeheerDetailScenarioFixture<KorteBeschrijvingWerdGewijzigdScenario> fixture)
    : BeheerDetailScenarioClassFixture<KorteBeschrijvingWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.KorteBeschrijving.Should()
               .BeEquivalentTo(fixture.Scenario.KorteBeschrijvingWerdGewijzigd.KorteBeschrijving);
    }
}
