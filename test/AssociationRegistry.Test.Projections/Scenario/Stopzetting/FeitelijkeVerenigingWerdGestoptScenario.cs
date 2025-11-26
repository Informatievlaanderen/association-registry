namespace AssociationRegistry.Test.Projections.Scenario.Stopzetting;

using Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;

public class FeitelijkeVerenigingWerdGestoptScenario : ScenarioBase
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VerenigingWerdGestopt VerenigingWerdGestopt;
    public readonly EinddatumWerdGewijzigd EinddatumWerdGewijzigd;

    public FeitelijkeVerenigingWerdGestoptScenario()
    {
        var startdatum = new DateOnly(year: 2023, month: 09, day: 06);
        var fixture = new Fixture().CustomizeAdminApi();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        VerenigingWerdGestopt = fixture.Create<VerenigingWerdGestopt>();

    }

    public override string AggregateId => FeitelijkeVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new EventsPerVCode(AggregateId, [FeitelijkeVerenigingWerdGeregistreerd, VerenigingWerdGestopt])
    ];
}
