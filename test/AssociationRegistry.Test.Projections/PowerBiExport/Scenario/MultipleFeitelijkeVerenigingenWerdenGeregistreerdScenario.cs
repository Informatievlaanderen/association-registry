namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AutoFixture;
using Events;
using System.Linq;

public class MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd[] VerenigingenwerdenGeregistreerd { get; }

    public MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario()
    {
        VerenigingenwerdenGeregistreerd = AutoFixture.CreateMany<FeitelijkeVerenigingWerdGeregistreerd>()
                                                     .ToArray();
    }

    public override string VCode => VerenigingenwerdenGeregistreerd[0].VCode;

    public override EventsPerVCode[] Events
        => VerenigingenwerdenGeregistreerd
          .Select(vereniging => new EventsPerVCode(vereniging.VCode, vereniging))
          .ToArray();
}
