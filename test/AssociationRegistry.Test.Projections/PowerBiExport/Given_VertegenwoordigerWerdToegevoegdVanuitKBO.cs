namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Events;
using Scenario.Vertegenwoordigers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdToegevoegdVanuitKBO(PowerBiScenarioFixture<VertegenwoordigerWerdToegevoegdVanuitKBOScenario> fixture)
    : PowerBiScenarioClassFixture<VertegenwoordigerWerdToegevoegdVanuitKBOScenario>
{
    [Fact]
    public void Vertegenwoordigers_Count_Is_Increased()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);
        fixture.Result.AantalVertegenwoordigers.Should().Be(1);
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.Historiek.Should().NotBeEmpty();
        fixture.Result.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(VertegenwoordigerWerdToegevoegdVanuitKBO));
    }
}
