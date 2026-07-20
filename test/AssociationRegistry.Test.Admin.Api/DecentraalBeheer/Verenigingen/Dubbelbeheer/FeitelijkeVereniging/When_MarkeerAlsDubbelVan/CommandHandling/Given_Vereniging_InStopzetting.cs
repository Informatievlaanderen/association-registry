namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using GrarConsumer.FusieEvents.When_Consuming_Merger_Events;
using Marten;
using Moq;
using Wolverine.Marten;
using Xunit;

public class Given_Vereniging_InStopzetting
{
    private readonly Fixture _fixture;
    private readonly VzerWerdInStopzettingGeplaatstScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;
    private readonly MarkeerAlsDubbelVanCommandHandler _commandHandler;
    private AanvaardDubbeleVerenigingMessage _outboxMessage;

    public Given_Vereniging_InStopzetting()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VzerWerdInStopzettingGeplaatstScenario();
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
    public async ValueTask Then_It_Saves_VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbel()
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
            new VerenigingWerdGemarkeerdAlsDubbelVan(_scenario.VCode, command.VCodeAuthentiekeVereniging),
            new VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbel()
        );
    }
}
