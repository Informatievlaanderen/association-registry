namespace AssociationRegistry.Test.Dubbels.When_Verwerk_Weigering_Dubbel_Door_Authentieke_Vereniging;

using Acties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Notifications;
using Vereniging;
using Xunit;

public class Given_Vereniging_Is_Not_Dubbel
{
    [Fact]
    public async Task Then_WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt_Event_Is_Saved()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
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
