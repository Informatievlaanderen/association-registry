namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using Events;
using Scenario.Bankrekeningnummers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_BankrekeningnummerWerdOvergenomenVanuitKBO(
    PowerBiScenarioFixture<BankrekeningnummerWerdOvergenomenVanuitKBOScenario> fixture
) : PowerBiScenarioClassFixture<BankrekeningnummerWerdOvergenomenVanuitKBOScenario>
{
    [Fact]
    public void Bron_Should_Be_Kbo()
    {
        fixture.Result.Bankrekeningnummers.Should().BeEquivalentTo([
            new Bankrekeningnummer(fixture.Scenario.BankrekeningnummerWerdOvergenomenVanuitKBO.BankrekeningnummerId,
                                   fixture.Scenario.BankrekeningnummerWerdToegevoegd.Doel,
                        [],
                                fixture.Scenario.BankrekeningnummerWerdOvergenomenVanuitKBO.Bron
                                   ),
        ]);
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
