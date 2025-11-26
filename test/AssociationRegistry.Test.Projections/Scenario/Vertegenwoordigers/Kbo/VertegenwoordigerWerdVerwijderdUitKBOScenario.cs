namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Kbo;

using AutoFixture;
using Events;

public class VertegenwoordigerWerdVerwijderdUitKBOScenario : InszScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdToegevoegdVanuitKBO Vertegenwoordiger1WerdToegevoegdVanuitKBO { get; }
    public VertegenwoordigerWerdToegevoegdVanuitKBO Vertegenwoordiger2WerdToegevoegdVanuitKBO { get; }
    public VertegenwoordigerWerdVerwijderdUitKBO Vertegenwoordiger1WerdVerwijderdUitKBO { get; }
    private string _insz { get; }

    public VertegenwoordigerWerdVerwijderdUitKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
        Vertegenwoordiger1WerdToegevoegdVanuitKBO = AutoFixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>();
        Vertegenwoordiger2WerdToegevoegdVanuitKBO = AutoFixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>();
        _insz = Vertegenwoordiger1WerdToegevoegdVanuitKBO.Insz;

        Vertegenwoordiger1WerdVerwijderdUitKBO = new VertegenwoordigerWerdVerwijderdUitKBO(
            Insz: Vertegenwoordiger1WerdToegevoegdVanuitKBO.Insz,
            VertegenwoordigerId: Vertegenwoordiger1WerdToegevoegdVanuitKBO.VertegenwoordigerId,
            Voornaam: Vertegenwoordiger1WerdToegevoegdVanuitKBO.Voornaam,
            Achternaam: Vertegenwoordiger1WerdToegevoegdVanuitKBO.Achternaam);
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            Vertegenwoordiger1WerdToegevoegdVanuitKBO,
            Vertegenwoordiger2WerdToegevoegdVanuitKBO,
            Vertegenwoordiger1WerdVerwijderdUitKBO),
    ];

    public override string Insz => _insz;
}
