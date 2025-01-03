namespace AssociationRegistry.Test.Dubbels.When_Verwerk_Weigering_Dubbel_Door_Authentieke_Vereniging;

using Acties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Notifications;
using Vereniging;
using Xunit;

public class Given_A_Dubbele_Vereniging
{
    [Fact]
    public async Task Then_WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt_Event_Is_Saved()
    {
        var scenario = new VerenigingWerdGemarkeerdAlsDubbelVanScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState(), true, true);
        var vCodeAuthentiekeVereniging = VCode.Create(scenario.VerenigingWerdGemarkeerdAlsDubbelVan.VCodeAuthentiekeVereniging);

        var command = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand(
            VCode: scenario.VCode,
            VCodeAuthentiekeVereniging: vCodeAuthentiekeVereniging);

        var sut = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler(
            repositoryMock,
            Mock.Of<INotifier>(),
            new NullLogger<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler>());

        await sut.Handle(command, CancellationToken.None);

        repositoryMock.ShouldHaveSaved(
            WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt.With(scenario.VCode, new VerenigingStatus.StatusDubbel(vCodeAuthentiekeVereniging, VerenigingStatus.Actief)));
    }
}
