namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using Framework;
using AutoFixture;
using Vereniging;

public class V019_AfdelingWerdGeregistreerd_WithMinimalFields : IEventsInDbScenario
{
    public readonly AfdelingWerdGeregistreerd AfdelingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V019_AfdelingWerdGeregistreerd_WithMinimalFields()
    {
        var fixture = new Fixture().CustomizeAll();

        KboNummerMoeder = fixture.Create<KboNummer>();
        VCode = "V9999019";
        AfdelingWerdGeregistreerd = fixture.Create<AfdelingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Moedervereniging = new AfdelingWerdGeregistreerd.MoederverenigingsData(KboNummerMoeder, string.Empty, $"Moeder {KboNummerMoeder}"),
            Locaties = Array.Empty<Registratiedata.Locatie>(),
            KorteNaam = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = Array.Empty<Registratiedata.Contactgegeven>(),
            Vertegenwoordigers = Array.Empty<Registratiedata.Vertegenwoordiger>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>(),
        };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string KboNummerMoeder { get; set; }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { AfdelingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
