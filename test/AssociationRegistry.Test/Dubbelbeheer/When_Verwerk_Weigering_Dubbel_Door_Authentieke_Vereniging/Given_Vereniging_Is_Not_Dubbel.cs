namespace AssociationRegistry.Test.Dubbelbeheer.When_Verwerk_Weigering_Dubbel_Door_Authentieke_Vereniging;

using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using Integrations.Slack;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class Given_Vereniging_Is_Not_Dubbel
{
    [Fact]
    public async ValueTask Then_WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt_Event_Is_Saved()
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
