namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AutoFixture;
using Events;

public class GeotagsWerdenBepaaldScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public GeotagsWerdenBepaald GeotagsWerdenBepaald { get; }

    public GeotagsWerdenBepaaldScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        GeotagsWerdenBepaald = AutoFixture.Create<GeotagsWerdenBepaald>() with{VCode = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode};
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, GeotagsWerdenBepaald),
    ];
}
