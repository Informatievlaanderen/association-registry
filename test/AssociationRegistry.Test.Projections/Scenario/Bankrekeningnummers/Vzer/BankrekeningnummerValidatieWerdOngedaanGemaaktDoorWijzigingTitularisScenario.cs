namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Vzer;

using AssociationRegistry.Events;
using AutoFixture;

public class BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularisScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public BankrekeningnummerWerdToegevoegd BankrekeningnummerWerdToegevoegd { get; }
    public BankrekeningnummerWerdGevalideerd BankrekeningnummerWerdGevalideerd { get; }
    public BankrekeningnummerWerdGewijzigd BankrekeningnummerWerdGewijzigd { get; }

    public BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis { get; }

    public BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularisScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        BankrekeningnummerWerdToegevoegd = AutoFixture.Create<BankrekeningnummerWerdToegevoegd>();

        BankrekeningnummerWerdGevalideerd = AutoFixture.Create<BankrekeningnummerWerdGevalideerd>() with
        {
            BankrekeningnummerId = BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
        };

        BankrekeningnummerWerdGewijzigd = AutoFixture.Create<BankrekeningnummerWerdGewijzigd>() with
        {
            BankrekeningnummerId = BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
        };

        BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis =
            AutoFixture.Create<BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis>() with
            {
                BankrekeningnummerId = BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
            };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                BankrekeningnummerWerdToegevoegd,
                BankrekeningnummerWerdGevalideerd,
                BankrekeningnummerWerdGewijzigd,
                BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis
            ),
        ];
}
