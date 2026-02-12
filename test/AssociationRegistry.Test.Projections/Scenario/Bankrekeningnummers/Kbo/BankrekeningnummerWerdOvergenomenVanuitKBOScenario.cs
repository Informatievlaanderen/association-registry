namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Kbo;

using AssociationRegistry.Events;
using AutoFixture;

public class BankrekeningnummerWerdOvergenomenVanuitKBOScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public BankrekeningnummerWerdToegevoegd BankrekeningnummerWerdToegevoegd { get; }
    public BankrekeningnummerWerdOvergenomenVanuitKBO BankrekeningnummerWerdOvergenomenVanuitKBO { get; }

    public BankrekeningnummerWerdOvergenomenVanuitKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
        BankrekeningnummerWerdToegevoegd = AutoFixture.Create<BankrekeningnummerWerdToegevoegd>();

        BankrekeningnummerWerdOvergenomenVanuitKBO =
            AutoFixture.Create<BankrekeningnummerWerdOvergenomenVanuitKBO>() with
            {
                BankrekeningnummerId = BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                Iban = BankrekeningnummerWerdToegevoegd.Iban,
            };
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
                BankrekeningnummerWerdToegevoegd,
                BankrekeningnummerWerdOvergenomenVanuitKBO
            ),
        ];
}
