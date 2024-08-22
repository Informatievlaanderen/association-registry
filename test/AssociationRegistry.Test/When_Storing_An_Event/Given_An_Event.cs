namespace AssociationRegistry.Test.When_Storing_An_Event;

using AssociationRegistry.Framework;
using Common.Framework;
using EventStore;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using NodaTime.Text;
using Xunit;
using Xunit.Categories;
using IEvent = Marten.Events.IEvent;

[IntegrationTest]
public class Given_An_Event
{
    [Fact]
    public async Task Then_it_is_persisted_in_the_database()
    {
        var documentStore = await TestDocumentStoreFactory.Create(nameof(Given_An_Event));
        // arrange
        var streamId = Guid.NewGuid().ToString();
        var someEvent = new SomeEvent("some event");
        var eventStore = new EventStore(documentStore, new EventConflictResolver(), NullLogger<EventStore>.Instance);

        // act
        var tijdstip = new Instant();
        var tijdstipString = InstantPattern.General.Format(tijdstip);

        await eventStore.Save(
            streamId,
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
    private record SomeEvent(string Name) : AssociationRegistry.Framework.IEvent;
}
