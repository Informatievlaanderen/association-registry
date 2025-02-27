namespace AssociationRegistry.Test.Projections.Beheer.Detail.Hoofdactiviteiten;

using Scenario.Hoofdactiviteiten;

[Collection(nameof(ProjectionContext))]
public class Given_HoofdactiviteitenVerenigingsloketWerdenGewijzigd(
    BeheerDetailScenarioFixture<HoofdactiviteitenVerenigingsloketWerdenGewijzigdScenario> fixture)
    : BeheerDetailScenarioClassFixture<HoofdactiviteitenVerenigingsloketWerdenGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.HoofdactiviteitenVerenigingsloket.Should()
               .BeEquivalentTo(fixture.Scenario.HoofdactiviteitenVerenigingsloketWerdenGewijzigd.HoofdactiviteitenVerenigingsloket);
    }
}
