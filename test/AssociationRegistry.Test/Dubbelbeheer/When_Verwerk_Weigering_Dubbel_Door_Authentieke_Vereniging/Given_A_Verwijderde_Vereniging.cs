namespace AssociationRegistry.Test.Dubbelbeheer.When_Verwerk_Weigering_Dubbel_Door_Authentieke_Vereniging;

using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using Events.Factories;
using Integrations.Slack;
using MartenDb.Store;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class Given_A_Verwijderde_Vereniging
{
    [Fact]
    public async ValueTask Then_WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt_Event_Is_Saved()
    {
        var scenario = new VerenigingWerdGemarkeerdAlsDubbelVanEnVerwijderdScenario();
        var aggregateSession = new AggregateSessionMock(scenario.GetVerenigingState(), true, true);
        var vCodeAuthentiekeVereniging = VCode.Create(
            scenario.VerenigingWerdGemarkeerdAlsDubbelVan.VCodeAuthentiekeVereniging
        );

        var command = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand(
            VCode: scenario.VCode,
            VCodeAuthentiekeVereniging: vCodeAuthentiekeVereniging
        );

        var sut = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler(
            aggregateSession,
            Mock.Of<INotifier>(),
            new NullLogger<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler>()
        );

        await sut.Handle(command, CancellationToken.None);

        aggregateSession.ShouldHaveSavedExact(
            EventFactory.WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(
                scenario.VCode,
                new VerenigingStatus.StatusDubbel(vCodeAuthentiekeVereniging, VerenigingStatus.Actief)
            )
        );

        aggregateSession.AssertLoadingDubbel();
        aggregateSession.AssertLoadingVerwijderd();
    }
}
