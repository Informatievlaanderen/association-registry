namespace AssociationRegistry.Test.Projections.VertegenwoordigersPerVCode;

using Admin.Schema.Vertegenwoordiger;
using Scenario.Registratie;
using Scenario.Vertegenwoordigers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden(
    VertegenwoordigersPerVCodeScenarioFixture<KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenScenario> fixture)
    : VertegenwoordigersPerVCodeScenarioClassFixture<KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenScenario>
{
    [Fact]
    public void VertegenwoordigersPerVCode_Document_Is_Saved()
        => fixture.Result!.VertegenwoordigersData
                  .First(x => x.VertegenwoordigerId == fixture.Scenario.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden.VertegenwoordigerId)
                  .Should()
                  .BeEquivalentTo(new VertegenwoordigerData(fixture.Scenario.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden.VertegenwoordigerId,
                                                                     VertegenwoordigerKszStatus.Overleden));
}
