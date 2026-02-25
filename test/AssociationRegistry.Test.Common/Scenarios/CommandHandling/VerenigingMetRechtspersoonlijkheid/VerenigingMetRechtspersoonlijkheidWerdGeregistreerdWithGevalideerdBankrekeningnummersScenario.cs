namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using global::AutoFixture;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGevalideerdBankrekeningnummersScenario
    : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009011");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO1EnWerdGevalideerd;
    public readonly BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO2;
    public readonly BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO3;
    public readonly BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO4;
    public readonly BankrekeningnummerWerdVerwijderdUitKBO BankrekeningnummerWerdVerwijderdUitKBO;
    public readonly AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd;
    private IEvent[] _events;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGevalideerdBankrekeningnummersScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        BankrekeningnummerWerdToegevoegdVanuitKBO1EnWerdGevalideerd =
            fixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>() with
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
            BankrekeningnummerWerdToegevoegdVanuitKBO4.Iban
        );

        AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd = fixture.Create<AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd>() with
        {
            BankrekeningnummerId = BankrekeningnummerWerdToegevoegdVanuitKBO1EnWerdGevalideerd.BankrekeningnummerId,
        };

        _events =
        [
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            BankrekeningnummerWerdToegevoegdVanuitKBO1EnWerdGevalideerd,
            BankrekeningnummerWerdToegevoegdVanuitKBO2,
            BankrekeningnummerWerdToegevoegdVanuitKBO3,
            BankrekeningnummerWerdToegevoegdVanuitKBO4,
            BankrekeningnummerWerdVerwijderdUitKBO,
            AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd,
        ];
    }

    public override IEnumerable<IEvent> Events() => _events;
}
