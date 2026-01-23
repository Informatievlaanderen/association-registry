namespace AssociationRegistry.Test.Dubbelbeheer.When_AanvaardDubbeleVereniging;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using MartenDb.Store;
using Moq;
using Wolverine;
using Xunit;

public class Given_Valid_AanvaardDubbeleVerenigingCommand
{
    [Fact]
    public async ValueTask Then_It_Saves_A_VerenigingAanvaarddeDubbeleVereniging_For_FeitelijkeVereniging()
    {
        var fixture = new Fixture().CustomizeDomain();
        var messageBus = new Mock<IMessageBus>();
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var aggregateSession = new AggregateSessionMock(scenario.GetVerenigingState());

        var command = fixture.Create<AanvaardDubbeleVerenigingCommand>() with { VCode = scenario.VCode };

        var sut = new AanvaardDubbeleVerenigingCommandHandler(aggregateSession, messageBus.Object);

        await sut.Handle(command, CancellationToken.None);

        aggregateSession.ShouldHaveSavedExact(
            new VerenigingAanvaarddeDubbeleVereniging(scenario.VCode, command.VCodeDubbeleVereniging)
        );

        messageBus.Verify(
            x =>
                x.InvokeAsync(
                    It.Is<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage>(y =>
                        y.VCode == command.VCodeDubbeleVereniging
                    ),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TimeSpan>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async ValueTask Then_It_Saves_A_VerenigingAanvaarddeDubbeleVereniging_For_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd()
    {
        var fixture = new Fixture().CustomizeDomain();
        var messageBus = new Mock<IMessageBus>();
        var scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        var aggregateSession = new AggregateSessionMock(scenario.GetVerenigingState());

        var command = fixture.Create<AanvaardDubbeleVerenigingCommand>() with { VCode = scenario.VCode };

        var sut = new AanvaardDubbeleVerenigingCommandHandler(aggregateSession, messageBus.Object);

        await sut.Handle(command, CancellationToken.None);

        aggregateSession.ShouldHaveSavedExact(
            new VerenigingAanvaarddeDubbeleVereniging(scenario.VCode, command.VCodeDubbeleVereniging)
        );

        messageBus.Verify(
            x =>
                x.InvokeAsync(
                    It.Is<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage>(y =>
                        y.VCode == command.VCodeDubbeleVereniging
                    ),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TimeSpan>()
                ),
            Times.Never
        );
    }
}
