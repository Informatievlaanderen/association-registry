namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Events;
using Scenario.Bankrekeningnummers.Kbo;
using Scenario.Bankrekeningnummers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningnummerWerdOvergenomenVanuitKBO(
    PowerBiScenarioFixture<BankrekeningnummerWerdOvergenomenVanuitKBOScenario> fixture
) : PowerBiScenarioClassFixture<BankrekeningnummerWerdOvergenomenVanuitKBOScenario>
{
    [Fact]
    public void AantalBankrekeningnummers_Should_Not_Be_Changed()
    {
        fixture.Result.AantalBankrekeningnummers.Should().Be(1);
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture
            .Result.Historiek.Should()
            .ContainSingle(x => x.EventType == nameof(BankrekeningnummerWerdOvergenomenVanuitKBO));
    }
}
