namespace AssociationRegistry.Test.When_Storing_A_DubbeleVerenigingWerdSamengevoegd_Event;

using AssociationRegistry.Admin.Schema.Persoonsgegevens;
using AssociationRegistry.EventStore.ConflictResolution;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.Framework;
using Marten;
using MartenDb.BankrekeningnummerPersoonsgegevens;
using MartenDb.Store;
using MartenDb.Transformers;
using MartenDb.Upcasters.Persoonsgegevens;
using MartenDb.VertegenwoordigerPersoonsgegevens;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using IEvent = AssociationRegistry.Events.IEvent;

/// <summary>
///     Een lege event store met transformers en upcasters, zoals in productie.
/// </summary>
public class PersoonsgegevensEventStore
{
    private readonly DocumentStore _store;

    private PersoonsgegevensEventStore(DocumentStore store)
    {
        _store = store;
    }

    public static async Task<PersoonsgegevensEventStore> Create(string schema)
    {
        DocumentStore store = null!;

        store = await TestDocumentStoreFactory.CreateAsync(
            schema,
            options => options.UpcastEvents(() => store.QuerySession())
        );

        return new PersoonsgegevensEventStore(store);
    }

    public async Task Save(string vCode, IEvent @event)
    {
        await using var session = _store.LightweightSession();

        var eventStore = new EventStore(
            session,
            new EventConflictResolver([new EmptyConflictResolutionStrategy()], [new EmptyConflictResolutionStrategy()]),
            new PersoonsgegevensProcessor(
                new PersoonsgegevensEventTransformers(),
                new VertegenwoordigerPersoonsgegevensRepository(
                    session,
                    new VertegenwoordigerPersoonsgegevensQuery(session)
                ),
                new BankrekeningnummerPersoonsgegevensRepository(
                    session,
                    new BankrekeningnummerPersoonsgegevensQuery(session)
                ),
                NullLogger<PersoonsgegevensProcessor>.Instance
            ),
            NullLogger<EventStore>.Instance
        );

        await eventStore.Save(
            vCode,
            EventStore.ExpectedVersion.NewStream,
            new CommandMetadata(Initiator: "OVO000000", new Instant(), Guid.NewGuid()),
            CancellationToken.None,
            @event
        );
    }

    /// <summary>Het event zoals het in de databank staat, zonder upcasting.</summary>
    public async Task<string> GetPersistedJson(string vCode)
    {
        await using var session = _store.QuerySession();

        var data = await session.AdvancedSql.QueryAsync<string>(
            $"select data::text from {_store.Options.Events.DatabaseSchemaName}.mt_events where stream_id = ?",
            CancellationToken.None,
            vCode
        );

        return data.Single();
    }

    public async Task<TEvent> GetUpcastedEvent<TEvent>(string vCode)
    {
        await using var session = _store.QuerySession();

        var events = await session.Events.FetchStreamAsync(vCode);

        return (TEvent)events.Single().Data;
    }

    public async Task<IReadOnlyList<VertegenwoordigerPersoonsgegevensDocument>> GetVertegenwoordigerPersoonsgegevens()
    {
        await using var session = _store.QuerySession();

        return await session.Query<VertegenwoordigerPersoonsgegevensDocument>().ToListAsync();
    }

    public async Task<IReadOnlyList<BankrekeningnummerPersoonsgegevensDocument>> GetBankrekeningnummerPersoonsgegevens()
    {
        await using var session = _store.QuerySession();

        return await session.Query<BankrekeningnummerPersoonsgegevensDocument>().ToListAsync();
    }

    public async Task DeletePersoonsgegevens(string vCode)
    {
        await using var session = _store.LightweightSession();

        session.DeleteWhere<VertegenwoordigerPersoonsgegevensDocument>(x => x.VCode == vCode);
        session.DeleteWhere<BankrekeningnummerPersoonsgegevensDocument>(x => x.VCode == vCode);

        await session.SaveChangesAsync();
    }
}
