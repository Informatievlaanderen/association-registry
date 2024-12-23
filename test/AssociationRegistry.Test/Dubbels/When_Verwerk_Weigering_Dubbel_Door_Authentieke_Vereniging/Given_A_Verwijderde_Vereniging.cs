namespace AssociationRegistry.Test.Dubbels.When_Verwerk_Weigering_Dubbel_Door_Authentieke_Vereniging;

using Acties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Moq;
using Vereniging;
using Xunit;

public class Given_A_Verwijderde_Vereniging
{
    [Fact]
    public async Task Then_WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt_Event_Is_Saved()
    {
        var scenario = new VerenigingWerdGemarkeerdAlsDubbelVanEnVerwijderdScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState(), true, true);
        var vCodeAuthentiekeVereniging = VCode.Create(scenario.VerenigingWerdGemarkeerdAlsDubbelVan.VCodeAuthentiekeVereniging);

        var command = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand(
            VCode: scenario.VCode,
            VCodeAuthentiekeVereniging: vCodeAuthentiekeVereniging);

        var sut = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler(repositoryMock);

        await sut.Handle(command, CancellationToken.None);

        repositoryMock.ShouldHaveSaved(
            WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt.With(scenario.VCode, vCodeAuthentiekeVereniging, VerenigingStatus.Actief));

        repositoryMock.AssertLoadingDubbel();
        repositoryMock.AssertLoadingVerwijderd();
    }
}
