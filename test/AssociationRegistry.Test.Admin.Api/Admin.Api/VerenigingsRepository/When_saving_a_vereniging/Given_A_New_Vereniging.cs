namespace AssociationRegistry.Test.Admin.Api.Admin.Api.VerenigingsRepository.When_saving_a_vereniging;

using AssociationRegistry.Admin.Api.EventStore;
using AssociationRegistry.ContactInfo;
using AssociationRegistry.Framework;
using AssociationRegistry.Locaties;
using AssociationRegistry.VCodes;
using AssociationRegistry.Vereniging;
using AssociationRegistry.VerenigingsNamen;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class EventStoreMock : IEventStore
{
    public record Invocation(string AggregateId, IEvent[] Events);

    public readonly List<Invocation> Invocations = new();

    public async Task<long> Save(string aggregateId, CommandMetadata metadata, params IEvent[] events)
    {
        Invocations.Add(new Invocation(aggregateId, events));
        return await Task.FromResult(-1);
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
        var vereniging = new Vereniging(vCode, naam, null, null, null, null, ContactLijst.Empty, LocatieLijst.Empty, DateOnly.FromDateTime(DateTime.Today));

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
