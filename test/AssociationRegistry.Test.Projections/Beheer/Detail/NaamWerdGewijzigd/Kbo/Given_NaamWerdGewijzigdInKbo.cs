namespace AssociationRegistry.Test.Projections.Beheer.Detail.NaamWerdGewijzigd.Kbo;

using Scenario.NaamWerdGewijzigd.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_NaamWerdGewijzigdInKbo(
    BeheerDetailScenarioFixture<NaamWerdGewijzigdInKboScenario> fixture)
    : BeheerDetailScenarioClassFixture<NaamWerdGewijzigdInKboScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Has_Naam_Gewijzigd()
    {
        fixture.Result.Naam.Should()
               .BeEquivalentTo(fixture.Scenario.NaamWerdGewijzigdInKbo.Naam);
    }
}
