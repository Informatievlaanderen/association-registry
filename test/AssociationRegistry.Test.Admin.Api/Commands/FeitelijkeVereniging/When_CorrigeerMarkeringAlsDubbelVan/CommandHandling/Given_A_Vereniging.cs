namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_CorrigeerMarkeringAlsDubbelVan.CommandHandling;

using Acties.CorrigeerAanvaardingDubbel;
using Acties.CorrigeerMarkeringAlsDubbelVan;
using Events;
using AssociationRegistry.Framework;
using Messages;
using GrarConsumer.FusieEvents.When_Consuming_Merger_Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Marten;
using Moq;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;
using VerenigingStatus = AssociationRegistry.Admin.Schema.Constants.VerenigingStatus;

[UnitTest]
public class Given_A_Vereniging
{
    private readonly Fixture _fixture;
    private readonly VerenigingWerdGemarkeerdAlsDubbelVanScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly CorrigeerMarkeringAlsDubbelVanCommandHandler _commandHandler;
    private CorrigeerAanvaardingDubbeleVerenigingMessage _outboxMessage;

    public Given_A_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingWerdGemarkeerdAlsDubbelVanScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var martenOutbox = new Mock<IMartenOutbox>();

        martenOutbox.CaptureOutboxSendAsyncMessage<CorrigeerAanvaardingDubbeleVerenigingMessage>(message => _outboxMessage = message);

        _commandHandler = new CorrigeerMarkeringAlsDubbelVanCommandHandler(
            _verenigingRepositoryMock,
            martenOutbox.Object,
            Mock.Of<IDocumentSession>()
        );
    }

    [Fact]
    public async Task Then_It_Saves_An_VerenigingWerdGermarkeerdAlsDubbel_Event()
    {
        var command = _fixture.Create<CorrigeerMarkeringAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
        };

        await _commandHandler.Handle(
            new CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(
            new MarkeringDubbeleVerengingWerdGecorrigeerd(
                _scenario.VCode,
                _scenario.VerenigingWerdGemarkeerdAlsDubbelVan.VCodeAuthentiekeVereniging,
                VerenigingStatus.Actief
            ));
    }

    [Fact]
    public async Task Then_It_Sends_A_Message_To_The_Outbox()
    {
        var command = _fixture.Create<CorrigeerMarkeringAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
        };

        await _commandHandler.Handle(
            new CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>()));

        _outboxMessage.Should()
                      .BeEquivalentTo(new CorrigeerAanvaardingDubbeleVerenigingMessage(_scenario.VerenigingWerdGemarkeerdAlsDubbelVan.VCodeAuthentiekeVereniging, command.VCode));
    }
}
