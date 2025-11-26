namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Kbo;

using AutoFixture;
using Events;

public class VertegenwoordigerWerdGewijzigdInKBOScenario : InszScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdToegevoegdVanuitKBO VertegenwoordigerWerdToegevoegdVanuitKBO { get; }
    public VertegenwoordigerWerdGewijzigdInKBO VertegenwoordigerWerdGewijzigdInKBO { get; }

    private string _insz { get; }
    public VertegenwoordigerWerdGewijzigdInKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
        VertegenwoordigerWerdToegevoegdVanuitKBO = AutoFixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>();
        _insz = VertegenwoordigerWerdToegevoegdVanuitKBO.Insz;

        VertegenwoordigerWerdGewijzigdInKBO = AutoFixture.Create<VertegenwoordigerWerdGewijzigdInKBO>() with
        {
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegdVanuitKBO.VertegenwoordigerId,
            Insz = VertegenwoordigerWerdToegevoegdVanuitKBO.Insz,
        };
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId,
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            VertegenwoordigerWerdToegevoegdVanuitKBO,
            VertegenwoordigerWerdGewijzigdInKBO),
    ];

    public override string Insz => _insz;
}
