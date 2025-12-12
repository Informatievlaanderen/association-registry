namespace AssociationRegistry.Test.Projections.VertegenwoordigersPerVCode;

using Admin.Schema.Vertegenwoordiger;
using Scenario.Registratie;
using Scenario.Vertegenwoordigers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_KszSyncHeeftVertegenwoordigerBevestigd(
    VertegenwoordigersPerVCodeScenarioFixture<KszSyncHeeftVertegenwoordigerBevestigdScenario> fixture)
    : VertegenwoordigersPerVCodeScenarioClassFixture<KszSyncHeeftVertegenwoordigerBevestigdScenario>
{
    [Fact]
    public void VertegenwoordigersPerVCode_Document_Is_Saved()
        => fixture.Result!.VertegenwoordigersData
                  .First(x => x.VertegenwoordigerId == fixture.Scenario.KszSyncHeeftVertegenwoordigerBevestigd.VertegenwoordigerId)
                  .Should()
                  .BeEquivalentTo(new VertegenwoordigerData(fixture.Scenario.KszSyncHeeftVertegenwoordigerBevestigd.VertegenwoordigerId,
                                                                     VertegenwoordigerKszStatus.Bevestigd));
}
