namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.EventStoreTests.When_storing_an_event;

using AssociationRegistry.Admin.Api.Events;
using FluentAssertions;
using Marten;
using Marten.Events;
using Microsoft.Extensions.Configuration;
using Xunit;
using IEvent = AssociationRegistry.Admin.Api.Events.IEvent;

public class Given_An_Event
{
    [Fact]
    public async Task Then_it_is_persisted_in_the_database()
    {
        // arrange
        var streamId = Guid.NewGuid().ToString();
        var eventStore = CreateEventStore();
        var someEvent = new SomeEvent("some event");

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

    private static EventStore CreateEventStore()
        => new(CreateDocumentStore());

    private static DocumentStore CreateDocumentStore()
        => DocumentStore.For(
            opts =>
            {
                opts.Connection(GetConnectionString());
                opts.Events.StreamIdentity = StreamIdentity.AsString;
            });

    private static async Task<IReadOnlyList<Marten.Events.IEvent>> GetEventsFromDb(string streamId)
    {
        var store = CreateDocumentStore();
        await using var session = store.LightweightSession();

        return await session.Events.FetchStreamAsync(streamId);
    }

    private static string GetConnectionString()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        Console.WriteLine(currentDirectory);
        var builder = new ConfigurationBuilder()
            .SetBasePath(currentDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);

        var configurationRoot = builder.Build();
        var connectionString = configurationRoot
                .GetValue<string>("eventstore_connectionstring");
        return connectionString;
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record SomeEvent(string Name) : IEvent;
}
