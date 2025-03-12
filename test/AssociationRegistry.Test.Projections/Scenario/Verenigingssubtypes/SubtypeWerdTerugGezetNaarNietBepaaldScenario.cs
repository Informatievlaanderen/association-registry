namespace AssociationRegistry.Test.Projections.Scenario.Verenigingssubtypes;

using AssociationRegistry.Events;
using AutoFixture;

public class SubtypeWerdTerugGezetNaarNietBepaaldScenario : InszScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    private VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging { get; }
    public SubtypeWerdTerugGezetNaarNietBepaald SubtypeWerdTerugGezetNaarNietBepaald { get; }

    public SubtypeWerdTerugGezetNaarNietBepaaldScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging =
            new VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode);

        SubtypeWerdTerugGezetNaarNietBepaald = new SubtypeWerdTerugGezetNaarNietBepaald(VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging.VCode);
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;
    public override string Insz => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.First().Insz;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging,
            SubtypeWerdTerugGezetNaarNietBepaald),
    ];
}
