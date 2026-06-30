namespace AssociationRegistry.Test.Projections.Scenario.Kbo;

using AutoFixture;
using Events;

public class KBOStatusWerdGecorrigeerdNaarActiefScenario : ScenarioBase
{
    public KBOStatusWerdGecorrigeerdNaarActiefScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        KBOStatusWerdGecorrigeerdNaarActief = AutoFixture.Create<KBOStatusWerdGecorrigeerdNaarActief>();
        VerenigingWerdGestoptInKBO = AutoFixture.Create<VerenigingWerdGestoptInKBO>();
    }

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public VerenigingWerdGestoptInKBO VerenigingWerdGestoptInKBO { get; set; }
    public KBOStatusWerdGecorrigeerdNaarActief KBOStatusWerdGecorrigeerdNaarActief { get; }
    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
                VerenigingWerdGestoptInKBO,
                KBOStatusWerdGecorrigeerdNaarActief
            ),
        ];
}
