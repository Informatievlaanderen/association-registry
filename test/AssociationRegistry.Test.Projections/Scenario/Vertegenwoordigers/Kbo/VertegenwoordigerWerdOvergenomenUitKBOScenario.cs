namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Kbo;

using AutoFixture;
using Events;

public class VertegenwoordigerWerdOvergenomenUitKBOScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdOvergenomenUitKBO VertegenwoordigerWerdOvergenomenUitKBO { get; }

    public VertegenwoordigerWerdOvergenomenUitKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
        VertegenwoordigerWerdOvergenomenUitKBO = AutoFixture.Create<VertegenwoordigerWerdOvergenomenUitKBO>();
    }

    public override string VCode => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, VertegenwoordigerWerdOvergenomenUitKBO),
    ];
}
