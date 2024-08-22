namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;

public class V015_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithAllFields_ForWijzigBasisgegevens : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V015_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithAllFields_ForWijzigBasisgegevens()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999015";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
            VCode,
            KboNummer: "0000000000",
            Rechtsvorm: "VZW",
            Naam: "VZW 0000000000",
            string.Empty,
            Startdatum: null);

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { VerenigingMetRechtspersoonlijkheidWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
