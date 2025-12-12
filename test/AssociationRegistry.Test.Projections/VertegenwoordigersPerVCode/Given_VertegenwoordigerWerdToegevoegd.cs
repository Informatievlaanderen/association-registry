namespace AssociationRegistry.Test.Projections.VertegenwoordigersPerVCode;

using Admin.Schema.Vertegenwoordiger;
using Scenario.Vertegenwoordigers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdToegevoegd(
    VertegenwoordigersPerVCodeScenarioFixture<VertegenwoordigerWerdToegevoegdScenario> fixture)
    : VertegenwoordigersPerVCodeScenarioClassFixture<VertegenwoordigerWerdToegevoegdScenario>
{
    [Fact]
    public void VertegenwoordigersPerVCode_Document_Is_Saved()
        => fixture.Result!.VertegenwoordigersData
                  .First(x => x.VertegenwoordigerId == fixture.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId)
                  .Should()
                  .BeEquivalentTo(new VertegenwoordigerData(fixture.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                                                                     VertegenwoordigerKszStatus.NogNietGesynced));
}
