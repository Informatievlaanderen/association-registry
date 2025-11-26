namespace AssociationRegistry.Test.Projections.Scenario.Verenigingssubtypes;

using Events;
using AutoFixture;

public class SubtypeWerdTerugGezetNaarNietBepaaldScenario : InszScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    private VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging { get; }
    public VerenigingssubtypeWerdTerugGezetNaarNietBepaald VerenigingssubtypeWerdTerugGezetNaarNietBepaald { get; }

    public SubtypeWerdTerugGezetNaarNietBepaaldScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging =
            new VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);

        VerenigingssubtypeWerdTerugGezetNaarNietBepaald = new VerenigingssubtypeWerdTerugGezetNaarNietBepaald(VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging.VCode);
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;
    public override string Insz => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.First().Insz;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging,
            VerenigingssubtypeWerdTerugGezetNaarNietBepaald),
    ];
}
