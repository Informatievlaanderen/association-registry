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
using MartenDb.Store;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class Given_Weigering_Reeds_Verwerkt_Door_DubbeleVereniging
{
    [Fact]
    public async ValueTask Then_ShouldNotHaveAnySaves()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario();
        var aggregateSession = new AggregateSessionMock(scenario.GetVerenigingState(), true, true);
        var command = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand(
            VCode: scenario.VCode,
            fixture.Create<VCode>()
        );

        var sut = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler(
            aggregateSession,
            Mock.Of<INotifier>(),
            new NullLogger<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler>()
        );

        await sut.Handle(command, CancellationToken.None);

        aggregateSession.ShouldNotHaveAnySaves();
    }
}
