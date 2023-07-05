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
        var fixture = new Fixture().CustomizeAdminApi();

        KboNummerMoeder = fixture.Create<KboNummer>();
        VCode = "V9999019";
        AfdelingWerdGeregistreerd = new AfdelingWerdGeregistreerd(
            VCode,
            "Antwerpse Bijscholing Clickers",
            new AfdelingWerdGeregistreerd.MoederverenigingsData("0123456789", string.Empty, "Moeder 0123456789"),
            string.Empty,
            string.Empty,
            null,
            Array.Empty<Registratiedata.Contactgegeven>(),
            Array.Empty<Registratiedata.Locatie>(),
            Array.Empty<Registratiedata.Vertegenwoordiger>(),
            new[] { new Registratiedata.HoofdactiviteitVerenigingsloket("BLA", "Buitengewoon Leuke Afkortingen") });
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
