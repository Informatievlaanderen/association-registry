namespace AssociationRegistry.Test.Projections.VertegenwoordigersPerVCode;

using Admin.Schema.Vertegenwoordiger;
using Scenario.Registratie;

[Collection(nameof(ProjectionContext))]
public class Given_FV_Geregistreerd(
    VertegenwoordigersPerVCodeScenarioFixture<FeitelijkeVerenigingWerdGeregistreerdScenario> fixture)
    : VertegenwoordigersPerVCodeScenarioClassFixture<FeitelijkeVerenigingWerdGeregistreerdScenario>
{
    [Fact]
    public void VertegenwoordigersPerVCode_Document_Is_Saved()
        => fixture.Result.VertegenwoordigersData.Should().BeEquivalentTo(
            fixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.Select(
                x => new VertegenwoordigerData(x.VertegenwoordigerId, VertegenwoordigerKszStatus.NogNietGesynced)));
}
