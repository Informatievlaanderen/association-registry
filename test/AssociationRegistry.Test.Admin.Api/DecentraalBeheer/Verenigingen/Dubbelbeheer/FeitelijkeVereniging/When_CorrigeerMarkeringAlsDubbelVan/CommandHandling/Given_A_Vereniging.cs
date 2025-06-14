namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_CorrigeerMarkeringAlsDubbelVan.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Dubbelbeheer.CorrigeerMarkeringAlsDubbelVan;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Messages;
using AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Marten;
using Moq;
using Wolverine.Marten;
using Xunit;
using VerenigingStatus = AssociationRegistry.Admin.Schema.Constants.VerenigingStatus;

public class Given_A_Vereniging
{
    private readonly Fixture _fixture;
    private readonly VerenigingWerdGemarkeerdAlsDubbelVanScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly CorrigeerMarkeringAlsDubbelVanCommandHandler _commandHandler;
    private AanvaardCorrectieDubbeleVerenigingMessage _outboxMessage;

    public Given_A_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingWerdGemarkeerdAlsDubbelVanScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState(), expectedLoadingDubbel: true);

        var martenOutbox = new Mock<IMartenOutbox>();

        martenOutbox.CaptureOutboxSendAsyncMessage<AanvaardCorrectieDubbeleVerenigingMessage>(message => _outboxMessage = message);

        _commandHandler = new CorrigeerMarkeringAlsDubbelVanCommandHandler(
            _verenigingRepositoryMock,
            martenOutbox.Object,
            Mock.Of<IDocumentSession>()
        );
    }

    [Fact]
    public async ValueTask Then_It_Saves_An_VerenigingWerdGermarkeerdAlsDubbel_Event()
    {
        var command = _fixture.Create<CorrigeerMarkeringAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
        };

        await _commandHandler.Handle(
            new CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new MarkeringDubbeleVerengingWerdGecorrigeerd(
                _scenario.VCode,
                _scenario.VerenigingWerdGemarkeerdAlsDubbelVan.VCodeAuthentiekeVereniging,
                VerenigingStatus.Actief
            ));
    }

    [Fact]
    public async ValueTask Then_It_Sends_A_Message_To_The_Outbox()
    {
        var command = _fixture.Create<CorrigeerMarkeringAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
        };

        await _commandHandler.Handle(
            new CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>()));

        _outboxMessage.Should()
                      .BeEquivalentTo(new AanvaardCorrectieDubbeleVerenigingMessage(_scenario.VerenigingWerdGemarkeerdAlsDubbelVan.VCodeAuthentiekeVereniging, command.VCode));
    }
}
