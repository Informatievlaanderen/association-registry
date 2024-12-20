namespace AssociationRegistry.Test.Dubbels.When_Verwerk_Weigering_Dubbel_Door_Authentieke_Vereniging;

using Acties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Xunit;

public class Given_Weigering_Reeds_Verwerkt_Door_DubbeleVereniging
{
    [Fact]
    public async Task Then_ShouldNotHaveAnySaves()
    {
        var scenario = new WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var command = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand(VCode: scenario.VCode);

        var sut = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler(repositoryMock);

        await sut.Handle(command, CancellationToken.None);

        repositoryMock.ShouldNotHaveAnySaves();
    }
}
