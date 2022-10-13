namespace AssociationRegistry.Test.Admin.Api.UnitTests.VerenigingsRepositoryTests.When_saving_a_vereniging;

using AssociationRegistry.Admin.Api.Events;
using AssociationRegistry.Admin.Api.Verenigingen;
using FluentAssertions;
using Xunit;

public class EventStoreMock : IEventStore
{
    public record Invocation(string AggregateId, IEvent[] Events);

    public readonly List<Invocation> Invocations = new();

    public async Task Save(string aggregateId, params IEvent[] events)
    {
        Invocations.Add(new Invocation(aggregateId, events));
        await Task.CompletedTask;
    }
}

public class Given_A_New_Vereniging
{
    [Fact]
    public async Task Then_the_verenigingcreatedevent_is_stored_in_the_EventStore()
    {
        var eventStore = new EventStoreMock();

        var repo = new VerenigingsRepository(eventStore);

        var vCode = "V000001";
        var naam = "Vereniging 1";
        var vereniging = new Vereniging(vCode, naam);

        await repo.Save(vereniging);

        eventStore.Invocations.Should().HaveCount(1);
        var invocation = eventStore.Invocations.Single();
        invocation.AggregateId.Should().Be(vCode);
        invocation.Events.Single().Should().BeEquivalentTo(new VerenigingWerdGeregistreerd(vCode, naam));
    }
}
