namespace AssociationRegistry.Test.Dubbelbeheer.When_Verwerk_Weigering_Dubbel_Door_Authentieke_Vereniging;

using AssociationRegistry.Notifications;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Dubbelbeheer.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class Given_Weigering_Reeds_Verwerkt_Door_DubbeleVereniging
{
    [Fact]
    public async Task Then_ShouldNotHaveAnySaves()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var command = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand(VCode: scenario.VCode, fixture.Create<VCode>());

        var sut = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler(
            repositoryMock,
            Mock.Of<INotifier>(),
            new NullLogger<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler>());

        await sut.Handle(command, CancellationToken.None);

        repositoryMock.ShouldNotHaveAnySaves();
    }
}
