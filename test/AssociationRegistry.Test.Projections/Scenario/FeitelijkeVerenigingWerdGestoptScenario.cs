namespace AssociationRegistry.Test.Projections.Scenario;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Events;

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

    public override string VCode => FeitelijkeVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new EventsPerVCode(VCode, [FeitelijkeVerenigingWerdGeregistreerd, VerenigingWerdGestopt])
    ];
}
