namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AutoFixture;
using global::AutoFixture;

public class V068_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen_For_AdresNietUniekInAdressenregister : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V068_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen_For_AdresNietUniekInAdressenregister()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999068";
        Naam = "Dee sjiekste club";

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            Locaties = fixture.CreateMany<Registratiedata.Locatie>().Select(
                (locatie, w) => locatie with
                {
                    IsPrimair = w == 0,
                    LocatieId = locatie.LocatieId,
                }
            ).ToArray(),
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }

    public DateOnly? Startdatum
        => FeitelijkeVerenigingWerdGeregistreerd.Startdatum;

    public IEvent[] GetEvents()
        => new IEvent[]
            { FeitelijkeVerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
