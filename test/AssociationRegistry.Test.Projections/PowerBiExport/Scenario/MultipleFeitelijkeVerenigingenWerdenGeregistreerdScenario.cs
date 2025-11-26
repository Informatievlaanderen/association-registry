namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AutoFixture;
using Events;

public class MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd[] VerenigingenwerdenGeregistreerd { get; }

    public MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario()
    {
        VerenigingenwerdenGeregistreerd = AutoFixture.CreateMany<FeitelijkeVerenigingWerdGeregistreerd>()
                                                     .ToArray();
    }

    public override string AggregateId => VerenigingenwerdenGeregistreerd[0].VCode;

    public override EventsPerVCode[] Events
        => VerenigingenwerdenGeregistreerd
          .Select(vereniging => new EventsPerVCode(vereniging.VCode, vereniging))
          .ToArray();
}
