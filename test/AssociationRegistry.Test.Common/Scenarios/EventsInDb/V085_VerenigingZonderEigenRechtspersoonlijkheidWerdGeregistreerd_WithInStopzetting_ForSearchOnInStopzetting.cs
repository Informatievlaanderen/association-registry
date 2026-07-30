namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using global::AutoFixture;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;

public class V085_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd_WithInStopzetting_ForSearchOnInStopzetting
    : IEventsInDbScenario
{
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VerenigingWerdInStopzettingGeplaatst VerenigingWerdInStopzettingGeplaatst;
    public readonly CommandMetadata Metadata;

    public V085_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd_WithInStopzetting_ForSearchOnInStopzetting()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999085";

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        VerenigingWerdInStopzettingGeplaatst = fixture.Create<VerenigingWerdInStopzettingGeplaatst>();

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }

    public IEvent[] GetEvents() =>
        new IEvent[]
        {
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingWerdInStopzettingGeplaatst,
        };

    public CommandMetadata GetCommandMetadata() => Metadata;
}
