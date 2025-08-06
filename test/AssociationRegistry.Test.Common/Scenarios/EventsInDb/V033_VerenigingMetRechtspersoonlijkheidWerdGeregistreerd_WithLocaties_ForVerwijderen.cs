namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;
using MartenDb.Store;

public class V033_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithLocaties_ForVerwijderen : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly LocatieWerdToegevoegd LocatieWerdToegevoegd;
    public readonly LocatieWerdToegevoegd LocatieWerdToegevoegd2;
    public readonly CommandMetadata Metadata;

    public V033_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithLocaties_ForVerwijderen()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999033";
        Naam = "Dee sjiekste club";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
        };

        LocatieWerdToegevoegd = new LocatieWerdToegevoegd(
            fixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = 1,
            });

        LocatieWerdToegevoegd2 = fixture.Create<LocatieWerdToegevoegd>();

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }

    public DateOnly? Startdatum
        => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            LocatieWerdToegevoegd,
            LocatieWerdToegevoegd2,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
