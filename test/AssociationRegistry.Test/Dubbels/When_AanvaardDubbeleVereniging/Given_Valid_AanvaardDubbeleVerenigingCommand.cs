namespace AssociationRegistry.Test.Dubbels.When_AanvaardDubbeleVereniging;

using AssociationRegistry.Acties.AanvaardDubbel;
using AssociationRegistry.Events;
using AssociationRegistry.Messages;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AutoFixture;
using Moq;
using Wolverine;
using Xunit;

public class Given_Valid_AanvaardDubbeleVerenigingCommand
{
    [Fact]
    public async Task Then_It_Saves_A_VerenigingAanvaarddeDubbeleVereniging_For_FeitelijkeVereniging()
    {
        var fixture = new Fixture().CustomizeDomain();
        var messageBus = new Mock<IMessageBus>();
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var command = fixture.Create<AanvaardDubbeleVerenigingCommand>()
            with
            {
                VCode = scenario.VCode,
            };

        var sut = new AanvaardDubbeleVerenigingCommandHandler(repositoryMock, messageBus.Object);

        await sut.Handle(command, CancellationToken.None);

        repositoryMock.ShouldHaveSaved(new VerenigingAanvaarddeDubbeleVereniging(scenario.VCode, command.VCodeDubbeleVereniging));

        messageBus.Verify(x => x.InvokeAsync(
                          It.Is<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage>(y => y.VCode == command.VCodeDubbeleVereniging),
                          It.IsAny<CancellationToken>(),
                          It.IsAny<TimeSpan>()),
                      Times.Never);
    }

    [Fact]
    public async Task Then_It_Saves_A_VerenigingAanvaarddeDubbeleVereniging_For_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd()
    {
        var fixture = new Fixture().CustomizeDomain();
        var messageBus = new Mock<IMessageBus>();
        var scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var command = fixture.Create<AanvaardDubbeleVerenigingCommand>()
            with
            {
                VCode = scenario.VCode,
            };

        var sut = new AanvaardDubbeleVerenigingCommandHandler(repositoryMock, messageBus.Object);

        await sut.Handle(command, CancellationToken.None);

        repositoryMock.ShouldHaveSaved(new VerenigingAanvaarddeDubbeleVereniging(scenario.VCode, command.VCodeDubbeleVereniging));

        messageBus.Verify(x => x.InvokeAsync(
                              It.Is<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage>(y => y.VCode == command.VCodeDubbeleVereniging),
                              It.IsAny<CancellationToken>(),
                              It.IsAny<TimeSpan>()),
                          Times.Never);
    }
}
