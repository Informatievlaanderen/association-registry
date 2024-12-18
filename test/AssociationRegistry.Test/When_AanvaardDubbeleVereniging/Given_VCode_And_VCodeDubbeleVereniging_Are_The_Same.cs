namespace AssociationRegistry.Test.When_AanvaardDubbeleVereniging;

using Acties.AanvaardDubbel;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Messages;
using Moq;
using Resources;
using Vereniging.Exceptions;
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

        var exception = await Assert.ThrowsAsync<InvalidOperationVerenigingKanGeenDubbelWordenVanZichzelf>(
            async () => await sut.Handle(command, CancellationToken.None));
        exception.Message.Should().Be(ExceptionMessages.VerenigingKanGeenDubbelWordenVanZichzelf);

        messageBus.Verify(x => x.InvokeAsync(
                              It.Is<CorrigeerMarkeerAlsDubbeleVerenigingMessage>(y => y.VCode == command.VCodeDubbeleVereniging),
                              It.IsAny<CancellationToken>(),
                              It.IsAny<TimeSpan?>()),
                          Times.Once);
    }
}
