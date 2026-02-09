namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Kbo;

using AssociationRegistry.Events;
using AutoFixture;

public class BankrekeningnummerWerdOvergenomenVanuitKBOScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public BankrekeningnummerWerdToegevoegd BankrekeningnummerWerdToegevoegd { get; }
    public BankrekeningnummerWerdOvergenomenVanuitKBO BankrekeningnummerWerdOvergenomenVanuitKBO { get; }

    public BankrekeningnummerWerdOvergenomenVanuitKBOScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        BankrekeningnummerWerdToegevoegd = AutoFixture.Create<BankrekeningnummerWerdToegevoegd>();

        BankrekeningnummerWerdOvergenomenVanuitKBO =
            AutoFixture.Create<BankrekeningnummerWerdOvergenomenVanuitKBO>() with
            {
                BankrekeningnummerId = BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                Iban = BankrekeningnummerWerdToegevoegd.Iban,
            };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                BankrekeningnummerWerdToegevoegd,
                BankrekeningnummerWerdOvergenomenVanuitKBO
            ),
        ];
}
