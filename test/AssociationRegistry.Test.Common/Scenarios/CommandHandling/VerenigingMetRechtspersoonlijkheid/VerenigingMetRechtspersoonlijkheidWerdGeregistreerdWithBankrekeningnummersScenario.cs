namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using global::AutoFixture;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009011");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO1;
    public readonly BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO2;
    public readonly BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO3;
    public readonly BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO4;
    public readonly BankrekeningnummerWerdVerwijderdUitKBO BankrekeningnummerWerdVerwijderdUitKBO;
    private IEvent[] _events;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
        };

        BankrekeningnummerWerdToegevoegdVanuitKBO1 = fixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>() with
        {
            BankrekeningnummerId = 1,
        };

        BankrekeningnummerWerdToegevoegdVanuitKBO2 = fixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>() with
        {
            BankrekeningnummerId = 2,
        };

        BankrekeningnummerWerdToegevoegdVanuitKBO3 = fixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>() with
        {
            BankrekeningnummerId = 3,
        };

        // add this one, so we can test nextId
        BankrekeningnummerWerdToegevoegdVanuitKBO4 = fixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>() with
        {
            BankrekeningnummerId = 4,
        };

        BankrekeningnummerWerdVerwijderdUitKBO = new BankrekeningnummerWerdVerwijderdUitKBO(
            4,
            BankrekeningnummerWerdToegevoegdVanuitKBO4.Iban);

        _events =
        [
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            BankrekeningnummerWerdToegevoegdVanuitKBO1,
            BankrekeningnummerWerdToegevoegdVanuitKBO2,
            BankrekeningnummerWerdToegevoegdVanuitKBO3,
            BankrekeningnummerWerdToegevoegdVanuitKBO4,
            BankrekeningnummerWerdVerwijderdUitKBO,
        ];
    }

    public override IEnumerable<IEvent> Events()
        => _events;
}
