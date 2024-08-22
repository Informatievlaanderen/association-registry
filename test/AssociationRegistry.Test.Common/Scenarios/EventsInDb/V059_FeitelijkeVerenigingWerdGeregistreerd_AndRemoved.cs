namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;

public class V059_FeitelijkeVerenigingWerdGeregistreerd_AndRemoved : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VerenigingWerdVerwijderd VerenigingWerdVerwijderd;
    public readonly CommandMetadata Metadata;

    public V059_FeitelijkeVerenigingWerdGeregistreerd_AndRemoved()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = VCode;

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = Array.Empty<Registratiedata.Locatie>(),
            KorteNaam = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = Array.Empty<Registratiedata.Contactgegeven>(),
            Vertegenwoordigers = Array.Empty<Registratiedata.Vertegenwoordiger>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>(),
        };

        VerenigingWerdVerwijderd = fixture.Create<VerenigingWerdVerwijderd>();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; } = "V9999059";
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { FeitelijkeVerenigingWerdGeregistreerd, VerenigingWerdVerwijderd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
