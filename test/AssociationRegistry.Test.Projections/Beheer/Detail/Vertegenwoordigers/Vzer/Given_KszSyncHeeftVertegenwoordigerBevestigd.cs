namespace AssociationRegistry.Test.Projections.Beheer.Detail.Vertegenwoordigers.Vzer;

using Scenario.Vertegenwoordigers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_KszSyncHeeftVertegenwoordigerBevestigd(
    BeheerDetailScenarioFixture<KszSyncHeeftVertegenwoordigerBevestigdScenario> fixture)
    : BeheerDetailScenarioClassFixture<KszSyncHeeftVertegenwoordigerBevestigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Vertegenwoordiger_Is_Niet_Verwijderd()
    {
        var vertegenwoordiger = fixture.Result.Vertegenwoordigers.SingleOrDefault(x => x.VertegenwoordigerId == fixture.Scenario.KszSyncHeeftVertegenwoordigerBevestigd.VertegenwoordigerId);

        vertegenwoordiger.Should().NotBeNull();
    }
}
