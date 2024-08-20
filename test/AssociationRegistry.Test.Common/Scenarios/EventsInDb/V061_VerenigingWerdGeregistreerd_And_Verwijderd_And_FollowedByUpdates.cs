namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AutoFixture;
using global::AutoFixture;

public class V061_VerenigingWerdGeregistreerd_And_Verwijderd_And_FollowedByUpdates : IEventsInDbScenario
{
    public readonly CommandMetadata Metadata;
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VerenigingWerdVerwijderd VerenigingWerdVerwijderd;

    public V061_VerenigingWerdGeregistreerd_And_Verwijderd_And_FollowedByUpdates()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = new[]
            {
                fixture.Create<Registratiedata.Locatie>() with
                {
                    Adres = fixture.Create<Registratiedata.Adres>(),
                },
            },
            Vertegenwoordigers = new[]
            {
                fixture.Create<Registratiedata.Vertegenwoordiger>() with
                {
                    VertegenwoordigerId = VertegenwoordigerId,
                },
            },
        };

        VerenigingWerdVerwijderd = fixture.Create<VerenigingWerdVerwijderd>();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; } = "V9999061";
    public int VertegenwoordigerId = 12345;
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { FeitelijkeVerenigingWerdGeregistreerd, VerenigingWerdVerwijderd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
