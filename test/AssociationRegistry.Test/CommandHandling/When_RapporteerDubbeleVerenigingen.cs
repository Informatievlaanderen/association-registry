namespace AssociationRegistry.Test.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.DubbelDetectie;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Events;
using Events.Factories;
using FluentAssertions;
using Marten;
using MartenDb.Store;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class When_RegistreerGedetecteerdeDubbels
{
    private readonly Fixture _fixture;
    private RapporteerDubbeleVerenigingenCommandHandler _sut;
    private Mock<IEventStore> _eventStore;
    private Events.IEvent[]? _capturedEvents;

    public When_RegistreerGedetecteerdeDubbels()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _eventStore = new Mock<IEventStore>();
        var duplicateVerenigingsRepository = new DubbelDetectieVerenigingsRepository(_eventStore.Object);

        _sut = new RapporteerDubbeleVerenigingenCommandHandler(duplicateVerenigingsRepository, Mock.Of<IDocumentSession>(),
                                                               NullLogger<RapporteerDubbeleVerenigingenCommandHandler>.Instance);

        _capturedEvents = null;

        _eventStore
           .Setup(s => s.SaveNew(
                      It.IsAny<string>(),
                      It.IsAny<IDocumentSession>(),
                      It.IsAny<CommandMetadata>(),
                      It.IsAny<CancellationToken>(),
                      It.IsAny<AssociationRegistry.Events.IEvent[]>()))
           .Callback<string, IDocumentSession, CommandMetadata, CancellationToken, AssociationRegistry.Events.IEvent[]>(
                (_, _, _, _, evts) => _capturedEvents = evts);
    }

    [Fact]
    public async Task Then_It_Saved_A_New_Stream_Of_GedeteceerdeDubbels()
    {
        var cmd  = _fixture.Create<RapporteerDubbeleVerenigingenCommand>();
        var meta = _fixture.Create<CommandMetadata>();
        var expectedEvent = EventFactory.DubbeleVerenigingenWerdenGedetecteerd(
            cmd.Key,
            cmd.Naam,
            cmd.Locaties,
            cmd.GedetecteerdeDubbels);

        await _sut.Handle(new CommandEnvelope<RapporteerDubbeleVerenigingenCommand>(cmd, meta), true);

        _capturedEvents.Should().NotBeNull();
        _capturedEvents!.Should().HaveCount(1);
        _capturedEvents[0].Should().BeOfType<DubbeleVerenigingenWerdenGedetecteerd>();

        var e = (DubbeleVerenigingenWerdenGedetecteerd)_capturedEvents[0];

        e.Key.Should().Be(expectedEvent.Key);
        e.Naam.Should().Be(expectedEvent.Naam);
        e.Locaties.Should().BeEquivalentTo(expectedEvent.Locaties);
        e.GedetecteerdeDubbels.Should().BeEquivalentTo(expectedEvent.GedetecteerdeDubbels);
    }
}
