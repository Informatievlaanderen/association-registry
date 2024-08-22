namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;

public class V020_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_ForDuplicateDetection : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V020_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_ForDuplicateDetection()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999020";
        Naam = "De coolste club";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }

    public DateOnly? Startdatum
        => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum;

    public IEvent[] GetEvents()
        => new IEvent[]
            { VerenigingMetRechtspersoonlijkheidWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
