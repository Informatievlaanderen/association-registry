namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using Events;
using Scenario.Bankrekeningnummers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningWerdToegevoegdVanuitKBO(
    PowerBiScenarioFixture<BankrekeningnummerWerdToegevoegdVanuitKBOScenario> fixture
) : PowerBiScenarioClassFixture<BankrekeningnummerWerdToegevoegdVanuitKBOScenario>
{
    [Fact]
    public void Bankrekeningnummer_Should_Be_Added()
    {
        fixture.Result.Bankrekeningnummers.Should().ContainEquivalentOf(new Bankrekeningnummer(
                                                                            fixture.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO.BankrekeningnummerId,
                                                                            string.Empty,
                                                                            [],
                                                                            fixture.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO.Bron
                                                                        ));
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture
            .Result.Historiek.Should()
            .ContainSingle(x => x.EventType == nameof(BankrekeningnummerWerdToegevoegdVanuitKBO));
    }
}
