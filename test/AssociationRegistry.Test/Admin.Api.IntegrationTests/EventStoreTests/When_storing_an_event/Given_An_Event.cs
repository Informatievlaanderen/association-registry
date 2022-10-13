namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.EventStoreTests.When_storing_an_event;

using Fixtures;
using FluentAssertions;
using Xunit;
using IEvent = AssociationRegistry.Admin.Api.Events.IEvent;

[Collection(VerenigingDbCollection.Name)]
public class Given_An_Event
{
    [Fact]
    public async Task Then_it_is_persisted_in_the_database()
    {
        // arrange
        var streamId = Guid.NewGuid().ToString();
        var someEvent = new SomeEvent("some event");
        var eventStore = await VerenigingDbFixture.CreateEventStore();

        // act
        await eventStore.Save(streamId, someEvent);

        // assert
        var events = await GetEventsFromDb(streamId);
        events.Should().HaveCount(1);
        var single = events.Single();
        single.Data.As<SomeEvent>().Should().BeEquivalentTo(someEvent);
        // verify assumptions about marten
        single.StreamKey.Should().Be(streamId);
        single.EventType.Should().Be<SomeEvent>();
    }

    private static async Task<IReadOnlyList<Marten.Events.IEvent>> GetEventsFromDb(string streamId)
    {
        using var documentStore = await VerenigingDbFixture.CreateDocumentStore();
        await using var session = documentStore.LightweightSession();

        return await session.Events.FetchStreamAsync(streamId);
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record SomeEvent(string Name) : IEvent;
}
