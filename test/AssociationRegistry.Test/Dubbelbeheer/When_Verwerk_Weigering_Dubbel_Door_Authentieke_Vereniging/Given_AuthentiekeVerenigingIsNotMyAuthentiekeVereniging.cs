namespace AssociationRegistry.Test.Dubbelbeheer.When_Verwerk_Weigering_Dubbel_Door_Authentieke_Vereniging;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using Integrations.Slack;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class Given_AuthentiekeVerenigingIsNotMyAuthentiekeVereniging
{
    [Fact]
    public async ValueTask Then_Throws_ApplicationException()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new VerenigingWerdGemarkeerdAlsDubbelVanScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState(), true, true);
        var notifier = new Mock<INotifier>();
        var vCodeAuthentiekeVereniging = VCode.Create(fixture.Create<VCode>());

        var command = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand(
            VCode: scenario.VCode,
            VCodeAuthentiekeVereniging: vCodeAuthentiekeVereniging);

        var sut = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler(
            repositoryMock,
            notifier.Object,
            new NullLogger<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler>());

        await Assert.ThrowsAsync<ApplicationException>(async () => await sut.Handle(command, CancellationToken.None));
    }
}
