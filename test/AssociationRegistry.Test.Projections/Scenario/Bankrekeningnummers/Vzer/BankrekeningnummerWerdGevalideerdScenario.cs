namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Vzer;

using AssociationRegistry.Events;
using AutoFixture;

public class BankrekeningnummerWerdGevalideerdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public BankrekeningnummerWerdToegevoegd BankrekeningnummerWerdToegevoegd { get; }
    public BankrekeningnummerWerdGevalideerd BankrekeningnummerWerdGevalideerd { get; }

    public BankrekeningnummerWerdGevalideerdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        BankrekeningnummerWerdToegevoegd = AutoFixture.Create<BankrekeningnummerWerdToegevoegd>();
        BankrekeningnummerWerdGevalideerd = AutoFixture.Create<BankrekeningnummerWerdGevalideerd>()
            with
            {
                BankrekeningnummerId = BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                Iban = BankrekeningnummerWerdToegevoegd.Iban,
                Titularis = BankrekeningnummerWerdToegevoegd.Titularis,
            };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, BankrekeningnummerWerdToegevoegd, BankrekeningnummerWerdGevalideerd),
    ];
}
