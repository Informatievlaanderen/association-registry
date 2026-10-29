namespace AssociationRegistry.Test.When_Storing_Events_For_Multiple_Aggregates;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.Framework;
using EventStore.ConflictResolution;
using FluentAssertions;
using Marten;
using MartenDb.BankrekeningnummerPersoonsgegevens;
using MartenDb.Store;
using MartenDb.Transformers;
using MartenDb.VertegenwoordigerPersoonsgegevens;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Persoonsgegevens;
using Xunit;

public class Given_Events_For_Two_Aggregates
{
    private static readonly CommandMetadata Metadata = new(Initiator: "SomeInitiator", new Instant(), Guid.NewGuid());

    [Fact]
    public async ValueTask Then_both_streams_are_persisted()
    {
        var documentStore = await TestDocumentStoreFactory.CreateAsync(nameof(Given_Events_For_Two_Aggregates));
        var firstStreamId = Guid.NewGuid().ToString();
        var secondStreamId = Guid.NewGuid().ToString();

        var results = await CreateEventStore(documentStore)
            .SaveTransactional(
                Metadata,
                CancellationToken.None,
                new AggregateEvents(firstStreamId, EventStore.ExpectedVersion.NewStream, [new SomeEvent("first")]),
                new AggregateEvents(secondStreamId, EventStore.ExpectedVersion.NewStream, [new SomeEvent("second")])
            );

        var firstStream = await GetEventsFromDb(firstStreamId, documentStore);
        var secondStream = await GetEventsFromDb(secondStreamId, documentStore);
        firstStream.Should().ContainSingle();
        secondStream.Should().ContainSingle();

        results
            .Should()
            .BeEquivalentTo(
                [
                    new StreamActionResult(firstStream.Single().Sequence, 1),
                    new StreamActionResult(secondStream.Single().Sequence, 1),
                ],
                options => options.WithStrictOrdering()
            );
    }

    [Fact]
    public async ValueTask With_A_Version_Conflict_On_One_Stream_Then_Nothing_Is_Persisted()
    {
        var documentStore = await TestDocumentStoreFactory.CreateAsync(nameof(Given_Events_For_Two_Aggregates));
        var firstStreamId = Guid.NewGuid().ToString();
        var conflictingStreamId = Guid.NewGuid().ToString();

        await CreateEventStore(documentStore)
            .Save(
                conflictingStreamId,
                EventStore.ExpectedVersion.NewStream,
                Metadata,
                CancellationToken.None,
                new SomeEvent("existing"),
                new SomeEvent("existing")
            );

        var act = () =>
            CreateEventStore(documentStore)
                .SaveTransactional(
                    Metadata,
                    CancellationToken.None,
                    new AggregateEvents(firstStreamId, EventStore.ExpectedVersion.NewStream, [new SomeEvent("first")]),
                    new AggregateEvents(conflictingStreamId, 1, [new SomeEvent("conflicting")])
                );

        await act.Should().ThrowAsync<UnexpectedAggregateVersionException>();
        (await GetEventsFromDb(firstStreamId, documentStore)).Should().BeEmpty();
        (await GetEventsFromDb(conflictingStreamId, documentStore)).Should().HaveCount(2);
    }

    [Fact]
    public async ValueTask With_A_Version_Conflict_Then_A_Retry_On_The_Same_Session_Only_Saves_The_New_Events()
    {
        var documentStore = await TestDocumentStoreFactory.CreateAsync(nameof(Given_Events_For_Two_Aggregates));
        var firstStreamId = Guid.NewGuid().ToString();
        var conflictingStreamId = Guid.NewGuid().ToString();

        await CreateEventStore(documentStore)
            .Save(
                conflictingStreamId,
                EventStore.ExpectedVersion.NewStream,
                Metadata,
                CancellationToken.None,
                new SomeEvent("existing")
            );

        var eventStore = CreateEventStore(documentStore);

        var conflictingSave = () =>
            eventStore.SaveTransactional(
                Metadata,
                CancellationToken.None,
                new AggregateEvents(firstStreamId, EventStore.ExpectedVersion.NewStream, [new SomeEvent("first")]),
                new AggregateEvents(conflictingStreamId, EventStore.ExpectedVersion.NewStream, [new SomeEvent("stale")])
            );

        await conflictingSave.Should().ThrowAsync<UnexpectedAggregateVersionException>();

        await eventStore.SaveTransactional(
            Metadata,
            CancellationToken.None,
            new AggregateEvents(firstStreamId, EventStore.ExpectedVersion.NewStream, [new SomeEvent("first")]),
            new AggregateEvents(conflictingStreamId, 1, [new SomeEvent("retried")])
        );

        (await GetEventsFromDb(firstStreamId, documentStore)).Should().ContainSingle();

        (await GetEventsFromDb(conflictingStreamId, documentStore))
            .Select(x => x.Data)
            .Should()
            .BeEquivalentTo(
                [new SomeEvent("existing"), new SomeEvent("retried")],
                options => options.WithStrictOrdering()
            );
    }

    private static EventStore CreateEventStore(IDocumentStore documentStore)
    {
        var session = documentStore.LightweightSession();

        return new EventStore(
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
    }

    private static async Task<IReadOnlyList<JasperFx.Events.IEvent>> GetEventsFromDb(
        string streamId,
        IDocumentStore documentStore
    )
    {
        await using var session = documentStore.LightweightSession();

        return await session.Events.FetchStreamAsync(streamId);
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record SomeEvent(string Name) : AssociationRegistry.Events.IEvent;
}
