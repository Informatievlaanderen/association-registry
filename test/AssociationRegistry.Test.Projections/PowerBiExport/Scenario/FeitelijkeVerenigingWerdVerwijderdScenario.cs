namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AutoFixture;
using Events;

public class FeitelijkeVerenigingWerdVerwijderdScenario :  ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd1 { get; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd2 { get; }
    public VerenigingWerdVerwijderd VerenigingWerdVerwijderd { get; }

    public FeitelijkeVerenigingWerdVerwijderdScenario()
    {
        FeitelijkeVerenigingWerdGeregistreerd1 = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        FeitelijkeVerenigingWerdGeregistreerd2 = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        VerenigingWerdVerwijderd =
            new VerenigingWerdVerwijderd(FeitelijkeVerenigingWerdGeregistreerd1.VCode, Reden: "Verwijderd voor testen.");
    }

    public override string AggregateId => FeitelijkeVerenigingWerdGeregistreerd1.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, FeitelijkeVerenigingWerdGeregistreerd1, VerenigingWerdVerwijderd),
        new(FeitelijkeVerenigingWerdGeregistreerd2.VCode, FeitelijkeVerenigingWerdGeregistreerd2),
    ];
}
