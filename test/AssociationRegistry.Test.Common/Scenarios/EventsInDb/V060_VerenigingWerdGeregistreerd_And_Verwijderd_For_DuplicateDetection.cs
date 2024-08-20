namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AutoFixture;
using global::AutoFixture;

public class V060_VerenigingWerdGeregistreerd_And_Verwijderd_For_DuplicateDetection : IEventsInDbScenario
{
    public readonly CommandMetadata Metadata;
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VerenigingWerdVerwijderd VerenigingWerdVerwijderd;

    public V060_VerenigingWerdGeregistreerd_And_Verwijderd_For_DuplicateDetection()
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
        };

        VerenigingWerdVerwijderd = fixture.Create<VerenigingWerdVerwijderd>();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; } = "V9999060";
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { FeitelijkeVerenigingWerdGeregistreerd, VerenigingWerdVerwijderd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
