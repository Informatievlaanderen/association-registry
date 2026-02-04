namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers.Kbo;

using AssociationRegistry.Events;
using AutoFixture;

public class BankrekeningnummerWerdGewijzigdKBOScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public BankrekeningnummerWerdToegevoegdVanuitKBO BankrekeningnummerWerdToegevoegdVanuitKBO { get; }
    public BankrekeningnummerWerdGewijzigd BankrekeningnummerWerdGewijzigd { get; }

    public BankrekeningnummerWerdGewijzigdKBOScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        BankrekeningnummerWerdToegevoegdVanuitKBO = AutoFixture.Create<BankrekeningnummerWerdToegevoegdVanuitKBO>();

        BankrekeningnummerWerdGewijzigd = AutoFixture.Create<BankrekeningnummerWerdGewijzigd>() with
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
                BankrekeningnummerWerdGewijzigd
            ),
        ];
}
