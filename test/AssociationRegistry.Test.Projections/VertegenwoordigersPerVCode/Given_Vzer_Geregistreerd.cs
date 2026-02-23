namespace AssociationRegistry.Test.Projections.VertegenwoordigersPerVCode;

using Admin.Schema.Vertegenwoordiger;
using Scenario.Registratie;

[Collection(nameof(ProjectionContext))]
public class Given_Vzer_Geregistreerd(
    VertegenwoordigersPerVCodeScenarioFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario> fixture
) : VertegenwoordigersPerVCodeScenarioClassFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario>
{
    [Fact]
    public void VertegenwoordigersPerVCode_Document_Is_Saved() =>
        fixture
            .Result!.VertegenwoordigersData.Should()
            .BeEquivalentTo(
                fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.Select(
                    x => new VertegenwoordigerData(
                        VertegenwoordigerId: x.VertegenwoordigerId,
                        Status: VertegenwoordigerKszStatus.NogNietGesynced
                    )
                )
            );
}

[Collection(nameof(ProjectionContext))]
public class Given_Vzer_Geregistreerd_Na_Eerste_Ksz_Event(
    VertegenwoordigersPerVCodeScenarioFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdNaEersteKszEventScenario> fixture
)
    : VertegenwoordigersPerVCodeScenarioClassFixture<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdNaEersteKszEventScenario>
{
    [Fact]
    public void VertegenwoordigersPerVCode_Document_Is_Saved() =>
        fixture
            .Result!.VertegenwoordigersData.Should()
            .BeEquivalentTo(
                fixture.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.Select(
                    x => new VertegenwoordigerData(
                        VertegenwoordigerId: x.VertegenwoordigerId,
                        Status: VertegenwoordigerKszStatus.Bevestigd
                    )
                )
            );
}
