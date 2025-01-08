namespace AssociationRegistry.Test.Dubbelbeheer.When_AanvaardDubbeleVereniging;

using AssociationRegistry.Messages;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using DecentraalBeheer.Dubbelbeheer.AanvaardDubbel;
using FluentAssertions;
using Moq;
using Wolverine;
using Xunit;

public class Given_VCode_And_VCodeDubbeleVereniging_Are_The_Same
{
    [Fact]
    public async Task Then_Throws_InvalidOperationVerenigingKanGeenDubbelWordenVanZichzelf()
    {
        var fixture = new Fixture().CustomizeDomain();
        var messageBus = new Mock<IMessageBus>();

        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var command = fixture.Create<AanvaardDubbeleVerenigingCommand>() with
        {
            VCode = scenario.VCode,
            VCodeDubbeleVereniging = scenario.VCode,
        };
        var sut = new AanvaardDubbeleVerenigingCommandHandler(repositoryMock, messageBus.Object);

        await sut.Handle(command, CancellationToken.None);

        messageBus.Verify(x => x.SendAsync(
                              It.Is<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage>(y => y.VCode == command.VCodeDubbeleVereniging),
                              It.IsAny<DeliveryOptions?>()),
                          Times.Once);
    }
}
