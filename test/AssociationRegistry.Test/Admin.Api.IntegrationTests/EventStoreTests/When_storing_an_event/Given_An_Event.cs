namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.EventStoreTests.When_storing_an_event;

using AssociationRegistry.Admin.Api.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Public.Api.Infrastructure;
using Fixtures;
using FluentAssertions;
using Marten;
using NodaTime.Extensions;
using Xunit;

public class Given_An_Event_Fixture : AdminApiFixture
{
    public Given_An_Event_Fixture() : base(nameof(Given_An_Event_Fixture))
    {
    }
}

public class Given_An_Event : IClassFixture<Given_An_Event_Fixture>
{
    private readonly AdminApiFixture _fixture;

    public Given_An_Event(Given_An_Event_Fixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    // TODO clear DB after test
    public async Task Then_it_is_persisted_in_the_database()
    {
        // arrange
        var streamId = Guid.NewGuid().ToString();
        var someEvent = new SomeEvent("some event");
        var eventStore = new EventStore(_fixture.DocumentStore);

        // act
        await eventStore.Save(streamId, new CommandMetadata("SomeInitiator", new DateTime(2022, 1, 1).ToUniversalTime().ToInstant()), someEvent);

        // assert
        var events = await GetEventsFromDb(streamId, _fixture.DocumentStore!);
        events.Should().HaveCount(1);
        var single = events.Single();
        single.Data.As<SomeEvent>().Should().BeEquivalentTo(someEvent);
        // verify assumptions about marten
        single.StreamKey.Should().Be(streamId);
        single.EventType.Should().Be<SomeEvent>();
        single.GetHeaderString(MetadataHeaderNames.Initiator).Should().Be("SomeInitiator");
        single.GetHeaderInstant(MetadataHeaderNames.Tijdstip).Should().Be(new DateTime(2022, 1, 1).ToUniversalTime().ToInstant());
    }

    private static async Task<IReadOnlyList<Marten.Events.IEvent>> GetEventsFromDb(string streamId, IDocumentStore documentStore)
    {
        await using var session = documentStore.OpenSession();

        return await session.Events.FetchStreamAsync(streamId);
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record SomeEvent(string Name) : IEvent;
}
