namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AutoFixture;
using Events;

public class WerkingsgebiedenWerdenGewijzigdScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public WerkingsgebiedenWerdenGewijzigd WerkingsgebiedenWerdenGewijzigd { get; set; }

    public WerkingsgebiedenWerdenGewijzigdScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        WerkingsgebiedenWerdenGewijzigd = AutoFixture.Create<WerkingsgebiedenWerdenGewijzigd>();
    }

    public override string VCode => VerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingWerdGeregistreerd, WerkingsgebiedenWerdenGewijzigd),
    ];
}
