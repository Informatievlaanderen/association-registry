namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using global::AutoFixture;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;

public class BankrekeningnummerWerdOvergenomenVanuitKBOScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009011");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO1;
    public readonly BankrekeningnummerWerdToegevoegd GIBankrekeningnummerWerdToegevoegd;
    public BankrekeningnummerWerdOvergenomenVanuitKBO BankrekeningnummerWerdOvergenomenVanuitKBO;

    private IEvent[] _events;

    public BankrekeningnummerWerdOvergenomenVanuitKBOScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        BankrekeningnummerWerdToegevoegdVanuitKBO1 = fixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>() with
        {
            BankrekeningnummerId = 1,
        };

        GIBankrekeningnummerWerdToegevoegd = fixture.Create<BankrekeningnummerWerdToegevoegd>() with
        {
            BankrekeningnummerId = 2,
        };

        BankrekeningnummerWerdOvergenomenVanuitKBO = fixture.Create<BankrekeningnummerWerdOvergenomenVanuitKBO>() with
        {
            BankrekeningnummerId = GIBankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
        };

        _events =
        [
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            BankrekeningnummerWerdToegevoegdVanuitKBO1,
            GIBankrekeningnummerWerdToegevoegd,
            BankrekeningnummerWerdOvergenomenVanuitKBO,
        ];
    }

    public override IEnumerable<IEvent> Events() => _events;
}
