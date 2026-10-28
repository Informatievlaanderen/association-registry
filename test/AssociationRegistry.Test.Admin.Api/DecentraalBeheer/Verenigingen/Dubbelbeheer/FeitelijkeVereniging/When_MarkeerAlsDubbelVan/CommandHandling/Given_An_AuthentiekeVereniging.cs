namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
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

/// <summary>
///     Een vereniging die eerder als authentiek fungeerde (A is dubbel van B) mag alsnog dubbel worden (B is dubbel van C).
/// </summary>
public class Given_An_Authentieke_Vereniging
{
    private readonly Fixture _fixture;
    private readonly VerenigingAanvaarddeDubbeleVerenigingScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;
    private readonly MarkeerAlsDubbelVanCommandHandler _commandHandler;
    private readonly MarkeerAlsDubbelVanCommand _command;
    private AanvaardDubbeleVerenigingMessage _outboxMessage = null!;

    public Given_An_Authentieke_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingAanvaarddeDubbeleVerenigingScenario(
            VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype.Feitelijke
        );
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        var martenOutbox = new Mock<IMartenOutbox>();
        martenOutbox.CaptureOutboxSendAsyncMessage<AanvaardDubbeleVerenigingMessage>(message =>
            _outboxMessage = message
        );

        _commandHandler = new MarkeerAlsDubbelVanCommandHandler(
            _aggregateSessionMock,
            new VerenigingsStateQueriesMock(),
            martenOutbox.Object,
            Mock.Of<IDocumentSession>()
        );

        _command = _fixture.Create<MarkeerAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
            VCodeAuthentiekeVereniging = _fixture.Create<VCode>(),
        };
    }

    private async Task Handle() =>
        await _commandHandler.Handle(
            new CommandEnvelope<MarkeerAlsDubbelVanCommand>(_command, _fixture.Create<CommandMetadata>())
        );

    [Fact]
    public async ValueTask Then_It_Saves_A_VerenigingWerdGemarkeerdAlsDubbelVan()
    {
        await Handle();

        _aggregateSessionMock.ShouldHaveSavedExact(
            new VerenigingWerdGemarkeerdAlsDubbelVan(_scenario.VCode, _command.VCodeAuthentiekeVereniging)
        );
    }

    [Fact]
    public async ValueTask Then_It_Asks_The_New_Authentieke_Vereniging_To_Aanvaard_The_Dubbel()
    {
        await Handle();

        _outboxMessage
            .Should()
            .BeEquivalentTo(new AanvaardDubbeleVerenigingMessage(_command.VCodeAuthentiekeVereniging, _scenario.VCode));
    }
}
