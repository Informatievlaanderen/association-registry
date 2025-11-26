namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AutoFixture;
using Events;

public class FeitelijkeVerenigingWerdGeregistreerdScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }

    public FeitelijkeVerenigingWerdGeregistreerdScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
    }

    public override string AggregateId => VerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingWerdGeregistreerd.VCode, VerenigingWerdGeregistreerd),
    ];
}
