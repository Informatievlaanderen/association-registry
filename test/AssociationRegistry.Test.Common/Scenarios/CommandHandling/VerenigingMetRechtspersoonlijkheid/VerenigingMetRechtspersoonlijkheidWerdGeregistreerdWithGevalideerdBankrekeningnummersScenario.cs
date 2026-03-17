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
    public readonly BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO;
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

        BankrekeningnummerWerdToegevoegdVanuitKBO =
            fixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>() with
            {
                BankrekeningnummerId = 1,
            };

        AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd = fixture.Create<AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd>() with
        {
            BankrekeningnummerId = BankrekeningnummerWerdToegevoegdVanuitKBO.BankrekeningnummerId,
        };

        _events =
        [
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            BankrekeningnummerWerdToegevoegdVanuitKBO,
            AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd,
        ];
    }

    public override IEnumerable<IEvent> Events() => _events;
}
