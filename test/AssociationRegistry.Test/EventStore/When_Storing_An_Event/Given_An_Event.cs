namespace AssociationRegistry.Test.When_Storing_An_Event;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.Framework;
using EventStore.ConflictResolution;
using FluentAssertions;
using JasperFx.Events;
using Marten;
using MartenDb.Store;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NodaTime;
using NodaTime.Text;
using Persoonsgegevens;
using Xunit;

public class Given_An_Event
{
    [Fact]
    public async ValueTask Then_it_is_persisted_in_the_database()
    {
        var documentStore = await TestDocumentStoreFactory.CreateAsync(nameof(Given_An_Event));
        // arrange
        var streamId = Guid.NewGuid().ToString();
        var someEvent = new SomeEvent("some event");
        var eventStore = new EventStore(documentStore.LightweightSession(), new EventConflictResolver([new EmptyConflictResolutionStrategy()], [new EmptyConflictResolutionStrategy()]), Mock.Of<IVertegenwoordigerPersoonsgegevensRepository>(), NullLogger<EventStore>.Instance);

        // act
        var tijdstip = new Instant();
        var tijdstipString = InstantPattern.General.Format(tijdstip);

        await eventStore.Save(
            streamId,
            0,
            new CommandMetadata(
                Initiator: "SomeInitiator",
                tijdstip,
                Guid.NewGuid()),
            CancellationToken.None,
            someEvent);

        // assert
        var events = await GetEventsFromDb(streamId, documentStore);
        events.Should().HaveCount(1);
        var single = events.Single();
        single.Data.As<SomeEvent>().Should().BeEquivalentTo(someEvent);
        // verify assumptions about marten
        single.StreamKey.Should().Be(streamId);
        single.EventType.Should().Be<SomeEvent>();
        single.GetHeaderString(MetadataHeaderNames.Initiator).Should().Be("SomeInitiator");
        single.GetHeaderString(MetadataHeaderNames.Tijdstip).Should().Be(tijdstipString);
    }

    private static async Task<IReadOnlyList<IEvent>> GetEventsFromDb(string streamId, IDocumentStore documentStore)
    {
        await using var session = documentStore.LightweightSession();

        return await session.Events.FetchStreamAsync(streamId);
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record SomeEvent(string Name) : AssociationRegistry.Events.IEvent;
}
