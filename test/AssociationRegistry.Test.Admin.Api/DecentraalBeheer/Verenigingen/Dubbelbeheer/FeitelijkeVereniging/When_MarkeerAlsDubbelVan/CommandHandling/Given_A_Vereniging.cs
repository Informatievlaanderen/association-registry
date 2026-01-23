namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Marten;
using Moq;
using Wolverine.Marten;
using Xunit;

public class Given_A_Vereniging
{
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;
    private readonly MarkeerAlsDubbelVanCommandHandler _commandHandler;
    private AanvaardDubbeleVerenigingMessage _outboxMessage;

    public Given_A_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        var martenOutbox = new Mock<IMartenOutbox>();
        martenOutbox.CaptureOutboxSendAsyncMessage<AanvaardDubbeleVerenigingMessage>(message =>
            _outboxMessage = message
        );

        var verenigingsStateQueriesMock = new VerenigingsStateQueriesMock();
        _commandHandler = new MarkeerAlsDubbelVanCommandHandler(
            _aggregateSessionMock,
            verenigingsStateQueriesMock,
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

        await _commandHandler.Handle(
            new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>())
        );

        _aggregateSessionMock.ShouldHaveSavedExact(
            new VerenigingWerdGemarkeerdAlsDubbelVan(_scenario.VCode, command.VCodeAuthentiekeVereniging)
        );
    }

    [Fact]
    public async ValueTask Then_It_Sends_A_Message_To_The_Outbox()
    {
        var command = _fixture.Create<MarkeerAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
            VCodeAuthentiekeVereniging = _fixture.Create<VCode>(),
        };

        await _commandHandler.Handle(
            new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>())
        );

        _outboxMessage
            .Should()
            .BeEquivalentTo(new AanvaardDubbeleVerenigingMessage(command.VCodeAuthentiekeVereniging, command.VCode));
    }
}
