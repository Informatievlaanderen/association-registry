namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Vzer;

using AssociationRegistry.Events;
using AutoFixture;

public class BankrekeningnummerWerdGevalideerdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public BankrekeningnummerWerdToegevoegd BankrekeningnummerWerdToegevoegd { get; }
    public AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd { get; }

    public BankrekeningnummerWerdGevalideerdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        BankrekeningnummerWerdToegevoegd = AutoFixture.Create<BankrekeningnummerWerdToegevoegd>();
        AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd = AutoFixture.Create<AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd>()
            with
            {
                BankrekeningnummerId = BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
            };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, BankrekeningnummerWerdToegevoegd, AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd),
    ];
}
