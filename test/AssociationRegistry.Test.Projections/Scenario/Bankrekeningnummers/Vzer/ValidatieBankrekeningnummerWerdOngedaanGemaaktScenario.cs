namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Vzer;

using AssociationRegistry.Events;
using AutoFixture;

public class ValidatieBankrekeningnummerWerdOngedaanGemaaktScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public BankrekeningnummerWerdToegevoegd BankrekeningnummerWerdToegevoegd { get; }

    public AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd
        AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd { get; }

    public AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt
        AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt { get; }

    public ValidatieBankrekeningnummerWerdOngedaanGemaaktScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        BankrekeningnummerWerdToegevoegd = AutoFixture.Create<BankrekeningnummerWerdToegevoegd>();

        AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd =
            AutoFixture.Create<AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd>()
                with
                {
                    BankrekeningnummerId = BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                };

        AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt =
            AutoFixture.Create<AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt>() with
            {
                BankrekeningnummerId = BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                OngedaanGemaaktDoor = AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd.BevestigdDoor,
            };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            BankrekeningnummerWerdToegevoegd,
            AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd,
            AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt),
    ];
}
