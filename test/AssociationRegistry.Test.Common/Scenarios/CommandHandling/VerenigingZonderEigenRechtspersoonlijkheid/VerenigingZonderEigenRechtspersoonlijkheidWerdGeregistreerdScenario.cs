namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Geotags;
using Events;
using Events.Factories;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public GeotagsWerdenBepaald? GeotagsWerdenBepaald { get; private set; }

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };
    }

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario WithGeotagsWerdenBepaald(
        GeotagsCollection geotags
    )
    {
        GeotagsWerdenBepaald = EventFactory.GeotagsWerdenBepaald(VCode, geotags);
        additionalEvents.Add(GeotagsWerdenBepaald);

        return this;
    }

    public readonly List<IEvent> additionalEvents = new();

    public override IEnumerable<IEvent> Events() =>
        new IEvent[] { VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd }.Concat(additionalEvents);
}
