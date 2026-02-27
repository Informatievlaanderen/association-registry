namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Kbo;

using AssociationRegistry.Events;
using AutoFixture;

public class BankrekeningnummerWerdGevalideerdKBOScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO { get; }
    public AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd { get; }

    public BankrekeningnummerWerdGevalideerdKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
        BankrekeningnummerWerdToegevoegdVanuitKBO = AutoFixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>();
        AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd = AutoFixture.Create<AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd>() with
        {
            BankrekeningnummerId = BankrekeningnummerWerdToegevoegdVanuitKBO.BankrekeningnummerId,
        };
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
                BankrekeningnummerWerdToegevoegdVanuitKBO,
                AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd
            ),
        ];
}
