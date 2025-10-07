namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Events;
using Scenario.Vertegenwoordigers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdVerwijderdUitKBO(PowerBiScenarioFixture<VertegenwoordigerWerdVerwijderdUitKBOScenario> fixture)
    : PowerBiScenarioClassFixture<VertegenwoordigerWerdVerwijderdUitKBOScenario>
{
    [Fact]
    public void Vertegenwoordigers_Count_Is_Decreased()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);
        fixture.Result.AantalVertegenwoordigers.Should().Be(1);
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.Historiek.Should().NotBeEmpty();
        fixture.Result.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(VertegenwoordigerWerdVerwijderdUitKBO));
    }
}
