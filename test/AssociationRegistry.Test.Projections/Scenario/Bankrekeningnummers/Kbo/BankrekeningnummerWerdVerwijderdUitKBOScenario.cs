namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Kbo;

using AssociationRegistry.Events;
using AutoFixture;

public class BankrekeningnummerWerdVerwijderdUitKBOScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO { get; }
    public BankrekeningnummerWerdVerwijderdUitKBO BankrekeningnummerWerdVerwijderdUitKBO { get; }

    public BankrekeningnummerWerdVerwijderdUitKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
        BankrekeningnummerWerdToegevoegdVanuitKBO = AutoFixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>();
        BankrekeningnummerWerdVerwijderdUitKBO = AutoFixture.Create<BankrekeningnummerWerdVerwijderdUitKBO>() with
        {
            BankrekeningnummerId = BankrekeningnummerWerdToegevoegdVanuitKBO.BankrekeningnummerId,
            Iban = BankrekeningnummerWerdToegevoegdVanuitKBO.Iban,
        };
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
                BankrekeningnummerWerdToegevoegdVanuitKBO,
                BankrekeningnummerWerdVerwijderdUitKBO
            ),
        ];
}
