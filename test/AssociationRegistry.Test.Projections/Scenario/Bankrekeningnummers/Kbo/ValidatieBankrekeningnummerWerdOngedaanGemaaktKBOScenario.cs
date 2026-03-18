namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Kbo;

using AssociationRegistry.Events;
using AutoFixture;

public class ValidatieBankrekeningnummerWerdOngedaanGemaaktKBOScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
    {
        get;
        set;
    }

    public BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO { get; }

    public AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd
        AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd { get; }

    public AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt
        AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt { get; }

    public ValidatieBankrekeningnummerWerdOngedaanGemaaktKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        BankrekeningnummerWerdToegevoegdVanuitKBO = AutoFixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>();

        AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd =
            AutoFixture.Create<AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd>() with
            {
                BankrekeningnummerId = BankrekeningnummerWerdToegevoegdVanuitKBO.BankrekeningnummerId,
            };

        AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt =
            AutoFixture.Create<AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt>() with
            {
                BankrekeningnummerId = BankrekeningnummerWerdToegevoegdVanuitKBO.BankrekeningnummerId,
                OngedaanGemaaktDoor = AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd.BevestigdDoor,
            };
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(
            AggregateId,
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            BankrekeningnummerWerdToegevoegdVanuitKBO,
            AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd,
            AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt
        ),
    ];
}
