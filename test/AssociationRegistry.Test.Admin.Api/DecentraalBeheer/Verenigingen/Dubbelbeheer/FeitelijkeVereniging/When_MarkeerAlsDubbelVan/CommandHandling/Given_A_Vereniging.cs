namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Dubbelbeheer.MarkeerAlsDubbelVan;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Messages;
using AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Marten;
using Moq;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Vereniging
{
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly MarkeerAlsDubbelVanCommandHandler _commandHandler;
    private AanvaardDubbeleVerenigingMessage _outboxMessage;

    public Given_A_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var martenOutbox = new Mock<IMartenOutbox>();
        martenOutbox.CaptureOutboxSendAsyncMessage<AanvaardDubbeleVerenigingMessage>(message => _outboxMessage = message);

        _commandHandler = new MarkeerAlsDubbelVanCommandHandler(
            _verenigingRepositoryMock,
            martenOutbox.Object,
            Mock.Of<IDocumentSession>()
        );
    }

    [Fact]
    public async ValueTask Then_It_Saves_An_VerenigingWerdGermarkeerdAlsDubbel_Event()
    {
        var command = _fixture.Create<MarkeerAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
            VCodeAuthentiekeVereniging = _fixture.Create<VCode>(),
        };

        await _commandHandler.Handle(new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(
            new VerenigingWerdGemarkeerdAlsDubbelVan(
                _scenario.VCode,
                command.VCodeAuthentiekeVereniging));
    }

    [Fact]
    public async ValueTask Then_It_Sends_A_Message_To_The_Outbox()
    {
        var command = _fixture.Create<MarkeerAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
            VCodeAuthentiekeVereniging = _fixture.Create<VCode>(),
        };

        await _commandHandler.Handle(new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>()));

        _outboxMessage.Should().BeEquivalentTo(new AanvaardDubbeleVerenigingMessage(command.VCodeAuthentiekeVereniging, command.VCode));
    }
}
