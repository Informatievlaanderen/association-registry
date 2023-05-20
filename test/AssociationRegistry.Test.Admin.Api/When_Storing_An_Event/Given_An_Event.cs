namespace AssociationRegistry.Test.Admin.Api.When_Storing_An_Event;

using EventStore;
using AssociationRegistry.Framework;
using Fixtures;
using FluentAssertions;
using Marten;
using NodaTime;
using NodaTime.Text;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_An_Event
{
    private readonly EventsInDbScenariosFixture _fixture;

    public Given_An_Event(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_it_is_persisted_in_the_database()
    {
        // arrange
        var streamId = Guid.NewGuid().ToString();
        var someEvent = new SomeEvent("some event");
        var eventStore = new EventStore(_fixture.DocumentStore);

        // act
        var tijdstip = new Instant();
        var tijdstipString = InstantPattern.General.Format(tijdstip);

        await eventStore.Save(
            streamId,
            new CommandMetadata(
                "SomeInitiator",
                tijdstip),
            someEvent);

        // assert
        var events = await GetEventsFromDb(streamId, _fixture.DocumentStore);
        events.Should().HaveCount(1);
        var single = events.Single();
        single.Data.As<SomeEvent>().Should().BeEquivalentTo(someEvent);
        // verify assumptions about marten
        single.StreamKey.Should().Be(streamId);
        single.EventType.Should().Be<SomeEvent>();
        single.GetHeaderString(MetadataHeaderNames.Initiator).Should().Be("SomeInitiator");
        single.GetHeaderString(MetadataHeaderNames.Tijdstip).Should().Be(tijdstipString);
    }

    private static async Task<IReadOnlyList<Marten.Events.IEvent>> GetEventsFromDb(string streamId, IDocumentStore documentStore)
    {
        await using var session = documentStore.OpenSession();

        return await session.Events.FetchStreamAsync(streamId);
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record SomeEvent(string Name) : IEvent;
}
