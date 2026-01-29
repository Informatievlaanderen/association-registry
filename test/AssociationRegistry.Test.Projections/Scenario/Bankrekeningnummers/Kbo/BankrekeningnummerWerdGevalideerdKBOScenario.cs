namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Kbo;

using AssociationRegistry.Events;
using AutoFixture;

public class BankrekeningnummerWerdGevalideerdKBOScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO { get; }
    public BankrekeningnummerWerdGevalideerd BankrekeningnummerWerdGevalideerd { get; }

    public BankrekeningnummerWerdGevalideerdKBOScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        BankrekeningnummerWerdToegevoegdVanuitKBO = AutoFixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>();
        BankrekeningnummerWerdGevalideerd = AutoFixture.Create<BankrekeningnummerWerdGevalideerd>()with
        {
            BankrekeningnummerId = BankrekeningnummerWerdToegevoegdVanuitKBO.BankrekeningnummerId,
            Iban = BankrekeningnummerWerdToegevoegdVanuitKBO.Iban,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, BankrekeningnummerWerdToegevoegdVanuitKBO, BankrekeningnummerWerdGevalideerd),
    ];
}
