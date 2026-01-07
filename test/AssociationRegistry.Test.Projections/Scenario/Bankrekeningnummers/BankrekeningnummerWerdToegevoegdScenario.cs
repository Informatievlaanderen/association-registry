namespace AssociationRegistry.Test.Projections.Scenario.Bankrekeningnummers;

using AutoFixture;
using Events;

public class BankrekeningnummerWerdToegevoegdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public BankrekeningnummerWerdToegevoegd BankrekeningnummerWerdToegevoegd { get; }

    public BankrekeningnummerWerdToegevoegdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        BankrekeningnummerWerdToegevoegd = AutoFixture.Create<BankrekeningnummerWerdToegevoegd>();
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, BankrekeningnummerWerdToegevoegd),
    ];
}
