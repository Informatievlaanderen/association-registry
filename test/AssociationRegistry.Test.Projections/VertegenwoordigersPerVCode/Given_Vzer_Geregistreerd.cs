namespace AssociationRegistry.Test.Projections.VertegenwoordigersPerVCode;

using Admin.Schema.Vertegenwoordiger;
using Scenario.Registratie;

[Collection(nameof(ProjectionContext))]
public class Given_Vzer_Geregistreerd(
    VertegenwoordigersPerVCodeScenarioFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario> fixture)
    : VertegenwoordigersPerVCodeScenarioClassFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario>
{
    [Fact]
    public void VertegenwoordigersPerVCode_Document_Is_Saved()
        => fixture.Result!.VertegenwoordigersData.Should().BeEquivalentTo(
            fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.Select(
                x => new VertegenwoordigerData(x.VertegenwoordigerId, VertegenwoordigerKszStatus.NogNietGesynced)));
}
