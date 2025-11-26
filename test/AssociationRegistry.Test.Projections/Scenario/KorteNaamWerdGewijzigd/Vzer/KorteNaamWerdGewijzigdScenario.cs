namespace AssociationRegistry.Test.Projections.Scenario.KorteNaamWerdGewijzigd.Vzer;

using Events;
using AutoFixture;

public class KorteNaamWerdGewijzigdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public KorteNaamWerdGewijzigd KorteNaamWerdGewijzigd { get; }

    public KorteNaamWerdGewijzigdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        KorteNaamWerdGewijzigd = AutoFixture.Create<KorteNaamWerdGewijzigd>() with
        {
            VCode = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            KorteNaamWerdGewijzigd
        ),
    ];
}
