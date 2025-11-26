namespace AssociationRegistry.Test.Projections.Scenario.Verenigingssubtypes;

using Events;
using AutoFixture;

public class VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario : InszScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingssubtypeWerdVerfijndNaarSubvereniging VerenigingssubtypeWerdVerfijndNaarSubvereniging { get; }

    public VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingssubtypeWerdVerfijndNaarSubvereniging =
            new VerenigingssubtypeWerdVerfijndNaarSubvereniging(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
               AutoFixture.Create<Registratiedata.SubverenigingVan>());
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;
    public override string Insz => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.First().Insz;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingssubtypeWerdVerfijndNaarSubvereniging),
    ];
}
