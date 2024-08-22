namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;

public class
    V069_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen_For_AdresWerdOvergenomenUitAdressenregister : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V069_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen_For_AdresWerdOvergenomenUitAdressenregister()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999069";
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
