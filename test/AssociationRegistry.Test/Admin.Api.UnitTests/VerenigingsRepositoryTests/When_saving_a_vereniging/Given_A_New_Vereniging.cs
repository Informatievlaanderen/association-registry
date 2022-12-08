namespace AssociationRegistry.Test.Admin.Api.UnitTests.VerenigingsRepositoryTests.When_saving_a_vereniging;

using AssociationRegistry.Admin.Api.Events;
using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Framework;
using AutoFixture;
using FluentAssertions;
using VCodes;
using Vereniging;
using VerenigingsNamen;
using Xunit;

public class EventStoreMock : IEventStore
{
    public record Invocation(string AggregateId, IEvent[] Events);

    public readonly List<Invocation> Invocations = new();

    public async Task Save(string aggregateId, CommandMetadata metadata, params IEvent[] events)    {
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

        var vCode = VCode.Create(1001);
        var naam = new VerenigingsNaam("Vereniging 1");
        var vereniging = new Vereniging(vCode, naam, null, null, null, null, DateOnly.FromDateTime(DateTime.Today));

        await repo.Save(vereniging, new Fixture().Create<CommandMetadata>());
        eventStore.Invocations.Should().HaveCount(1);
        var invocation = eventStore.Invocations.Single();
        invocation.AggregateId.Should().Be(vCode);

        var theEvent = (VerenigingWerdGeregistreerd)invocation.Events.Single();

        theEvent.VCode.Should().Be("V0001001");
        theEvent.Naam.Should().Be("Vereniging 1");
        theEvent.KorteNaam.Should().BeNull();
        theEvent.KorteBeschrijving.Should().BeNull();
        theEvent.KboNummer.Should().BeNull();
        theEvent.Startdatum.Should().BeNull();
    }
}
