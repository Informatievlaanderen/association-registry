namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Vzer;

using AssociationRegistry.Events;
using AutoFixture;

public class BankrekeningnummerWerdVerwijderdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public BankrekeningnummerWerdToegevoegd BankrekeningnummerWerdToegevoegd { get; }
    public BankrekeningnummerWerdVerwijderd BankrekeningnummerWerdVerwijderd { get; }

    public BankrekeningnummerWerdVerwijderdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        BankrekeningnummerWerdToegevoegd = AutoFixture.Create<BankrekeningnummerWerdToegevoegd>();
        BankrekeningnummerWerdVerwijderd = AutoFixture.Create<BankrekeningnummerWerdVerwijderd>()
            with
            {
                BankrekeningnummerId = BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                Iban = BankrekeningnummerWerdToegevoegd.Iban,
            };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, BankrekeningnummerWerdToegevoegd, BankrekeningnummerWerdVerwijderd),
    ];
}
