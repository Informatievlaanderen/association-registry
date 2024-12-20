namespace AssociationRegistry.Test.When_Verwerk_Weigering_Dubbel_Door_Authentieke_Vereniging;

using Acties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;

public class Given_A_Dubbele_Vereniging
{
    [Fact]
    public async Task Then_WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt_Event_Is_Saved()
    {
        var scenario = new VerenigingWerdGemarkeerdAlsDubbelVanScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var command = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand(VCode: scenario.VCode);

        var sut = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler(repositoryMock);

        await sut.Handle(command, CancellationToken.None);

        repositoryMock.ShouldHaveSaved(new WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(scenario.VCode));
    }
}

