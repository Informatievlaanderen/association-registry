namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AutoFixture;
using global::AutoFixture;

public class V026_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V026_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999026";
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
                    AdresId = null
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
