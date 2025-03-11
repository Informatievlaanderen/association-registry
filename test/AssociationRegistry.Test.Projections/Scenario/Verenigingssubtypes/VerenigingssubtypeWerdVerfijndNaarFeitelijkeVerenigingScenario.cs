namespace AssociationRegistry.Test.Projections.Scenario.Verenigingssubtypes;

using AssociationRegistry.Events;
using AutoFixture;

public class VerenigingssubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario : InszScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging { get; }

    public VerenigingssubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging =
            new VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;
    public override string Insz => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.First().Insz;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging),
    ];
}
