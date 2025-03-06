namespace AssociationRegistry.Test.Dubbelbeheer.When_AanvaardDubbeleVereniging;

using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using DecentraalBeheer.Dubbelbeheer.AanvaardDubbel;
using FluentAssertions;
using Moq;
using Resources;
using System.Threading;
using System.Threading.Tasks;
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

        var exception = await Assert.ThrowsAsync<InvalidOperationVerenigingKanGeenDubbelWordenVanZichzelf>(async () => await sut.Handle(command, CancellationToken.None)) ;

        exception.Message.Should().Be(ExceptionMessages.VerenigingKanGeenDubbelWordenVanZichzelf);
    }
}
