namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Kbo;

using AssociationRegistry.Events;
using AutoFixture;

public class BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularisKBOScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO { get; }
    public BankrekeningnummerWerdGevalideerd BankrekeningnummerWerdGevalideerd { get; }
    public BankrekeningnummerWerdGewijzigd BankrekeningnummerWerdGewijzigd { get; }

    public BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis { get; }

    public BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularisKBOScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        BankrekeningnummerWerdToegevoegdVanuitKBO = AutoFixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>();

        BankrekeningnummerWerdGevalideerd = AutoFixture.Create<BankrekeningnummerWerdGevalideerd>() with
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

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                BankrekeningnummerWerdToegevoegdVanuitKBO,
                BankrekeningnummerWerdGevalideerd,
                BankrekeningnummerWerdGewijzigd,
                BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis
            ),
        ];
}
