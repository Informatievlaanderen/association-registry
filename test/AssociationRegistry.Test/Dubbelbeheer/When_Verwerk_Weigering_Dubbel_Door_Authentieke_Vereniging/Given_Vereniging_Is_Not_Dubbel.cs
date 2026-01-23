namespace AssociationRegistry.Test.Dubbelbeheer.When_Verwerk_Weigering_Dubbel_Door_Authentieke_Vereniging;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using Integrations.Slack;
using MartenDb.Store;
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
