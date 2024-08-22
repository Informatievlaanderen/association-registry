namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;

public class V032_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithLocaties_ForWijzigen : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly LocatieWerdToegevoegd LocatieWerdToegevoegd;
    public readonly CommandMetadata Metadata;

    public V032_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithLocaties_ForWijzigen()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999032";
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
                AdresId = null,
            });

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
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
