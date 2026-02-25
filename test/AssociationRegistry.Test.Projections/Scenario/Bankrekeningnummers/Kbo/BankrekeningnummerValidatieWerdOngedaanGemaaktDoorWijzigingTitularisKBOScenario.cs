namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Kbo;

using AssociationRegistry.Events;
using AutoFixture;

public class BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularisKBOScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO { get; }
    public AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd { get; }
    public BankrekeningnummerWerdGewijzigd BankrekeningnummerWerdGewijzigd { get; }

    public BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis { get; }

    public BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularisKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        BankrekeningnummerWerdToegevoegdVanuitKBO = AutoFixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>();

        AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd = AutoFixture.Create<AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd>() with
        {
            BankrekeningnummerId = BankrekeningnummerWerdToegevoegdVanuitKBO.BankrekeningnummerId,
        };

        BankrekeningnummerWerdGewijzigd = AutoFixture.Create<BankrekeningnummerWerdGewijzigd>() with
        {
            BankrekeningnummerId = BankrekeningnummerWerdToegevoegdVanuitKBO.BankrekeningnummerId,
        };

        BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis =
            AutoFixture.Create<BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis>() with
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
                AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd,
                BankrekeningnummerWerdGewijzigd,
                BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis
            ),
        ];
}
