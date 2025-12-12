namespace AssociationRegistry.Test.Projections.VertegenwoordigersPerVCode;

using Admin.Schema.Vertegenwoordiger;
using Scenario.Registratie;
using Scenario.Vertegenwoordigers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend(
    VertegenwoordigersPerVCodeScenarioFixture<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendScenario> fixture)
    : VertegenwoordigersPerVCodeScenarioClassFixture<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendScenario>
{
    [Fact]
    public void VertegenwoordigersPerVCode_Document_Is_Saved()
        => fixture.Result!.VertegenwoordigersData
                  .First(x => x.VertegenwoordigerId == fixture.Scenario.KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend.VertegenwoordigerId)
                  .Should()
                  .BeEquivalentTo(new VertegenwoordigerData(fixture.Scenario.KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend.VertegenwoordigerId,
                                                                     VertegenwoordigerKszStatus.NietGekend));
}
