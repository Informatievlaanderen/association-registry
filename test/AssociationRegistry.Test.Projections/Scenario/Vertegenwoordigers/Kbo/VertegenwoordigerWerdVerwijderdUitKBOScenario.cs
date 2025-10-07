namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Kbo;

using AutoFixture;
using Events;

public class VertegenwoordigerWerdVerwijderdUitKBOScenario : InszScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdToegevoegdVanuitKBO VertegenwoordigerWerdToegevoegdVanuitKBO { get; }
    public VertegenwoordigerWerdVerwijderdUitKBO VertegenwoordigerWerdVerwijderdUitKBO { get; }
    private string _insz { get; }

    public VertegenwoordigerWerdVerwijderdUitKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
        VertegenwoordigerWerdToegevoegdVanuitKBO = AutoFixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>();
        _insz = VertegenwoordigerWerdToegevoegdVanuitKBO.Insz;

        VertegenwoordigerWerdVerwijderdUitKBO = new VertegenwoordigerWerdVerwijderdUitKBO(
            Insz: VertegenwoordigerWerdToegevoegdVanuitKBO.Insz,
            VertegenwoordigerId: VertegenwoordigerWerdToegevoegdVanuitKBO.VertegenwoordigerId,
            Voornaam: VertegenwoordigerWerdToegevoegdVanuitKBO.Voornaam,
            Achternaam: VertegenwoordigerWerdToegevoegdVanuitKBO.Achternaam);
    }

    public override string VCode => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            VertegenwoordigerWerdToegevoegdVanuitKBO,
            VertegenwoordigerWerdVerwijderdUitKBO),
    ];

    public override string Insz => _insz;
}
