namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AutoFixture;
using Events;

public class HoofdactiviteitenWerdenGewijzigdScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public HoofdactiviteitenVerenigingsloketWerdenGewijzigd HoofdactiviteitenVerenigingsloketWerdenGewijzigd { get; set; }

    public HoofdactiviteitenWerdenGewijzigdScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        HoofdactiviteitenVerenigingsloketWerdenGewijzigd = AutoFixture.Create<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>();
    }

    public override string VCode => VerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingWerdGeregistreerd, HoofdactiviteitenVerenigingsloketWerdenGewijzigd),
    ];
}
