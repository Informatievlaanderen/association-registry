namespace AssociationRegistry.Test.Admin.Api.UnitTests.VerenigingsRepositoryTests.When_saving_a_vereniging;

using AssociationRegistry.Admin.Api.Events;
using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Admin.Api.Verenigingen.VCodes;
using AssociationRegistry.Admin.Api.Verenigingen.VerenigingsNamen;
using AssociationRegistry.Framework;
using FluentAssertions;
using Vereniging;
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

        var vCode = new VCode(1);
        var naam = new VerenigingsNaam("Vereniging 1");
        var vereniging = new Vereniging(vCode, naam, null, null, null, null, DateOnly.FromDateTime(DateTime.Today));

        await repo.Save(vereniging);

        eventStore.Invocations.Should().HaveCount(1);
        var invocation = eventStore.Invocations.Single();
        invocation.AggregateId.Should().Be(vCode);

        var theEvent = (VerenigingWerdGeregistreerd)invocation.Events.Single();

        theEvent.VCode.Should().Be("V000001");
        theEvent.Naam.Should().Be("Vereniging 1");
    }
}
