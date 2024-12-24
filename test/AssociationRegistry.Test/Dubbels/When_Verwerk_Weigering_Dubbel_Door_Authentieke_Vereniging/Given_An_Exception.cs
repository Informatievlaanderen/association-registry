﻿namespace AssociationRegistry.Test.Dubbels.When_Verwerk_Weigering_Dubbel_Door_Authentieke_Vereniging;

using Acties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using Common.Scenarios.CommandHandling;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Notifications;
using Notifications.Messages;
using Vereniging;
using Xunit;

public class Given_An_Exception
{
    [Fact]
    public async Task Then_It_Retries()
    {
        var scenario = new VerenigingWerdGemarkeerdAlsDubbelVanScenario();
        var repositoryMock = new Mock<IVerenigingsRepository>();
        var notifier = new Mock<INotifier>();
        var vCodeAuthentiekeVereniging = VCode.Create(scenario.VerenigingWerdGemarkeerdAlsDubbelVan.VCodeAuthentiekeVereniging);

        var command = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand(
            VCode: scenario.VCode,
            VCodeAuthentiekeVereniging: vCodeAuthentiekeVereniging);

        var sut = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler(
            repositoryMock.Object,
            notifier.Object,
            new NullLogger<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler>());

        // ignore the assert throw, only interested in the veryfies
        await Assert.ThrowsAsync<NullReferenceException>(async () => await sut.Handle(command, CancellationToken.None));

        repositoryMock.Verify(x => x.Load<Vereniging>(It.IsAny<VCode>(), It.IsAny<int?>(), true, true), times: Times.Exactly(5));
        notifier.Verify(x => x.Notify(It.IsAny<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingGefaald>()), times: Times.Exactly(4));
    }
}