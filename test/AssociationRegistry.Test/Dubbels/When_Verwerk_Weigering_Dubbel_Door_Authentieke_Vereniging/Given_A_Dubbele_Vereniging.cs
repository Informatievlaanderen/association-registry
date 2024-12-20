namespace AssociationRegistry.Test.Dubbels.When_Verwerk_Weigering_Dubbel_Door_Authentieke_Vereniging;

using AssociationRegistry.Acties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using Vereniging;
using Xunit;

public class Given_A_Dubbele_Vereniging
{
    [Fact]
    public async Task Then_WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt_Event_Is_Saved()
    {
        var scenario = new VerenigingWerdGemarkeerdAlsDubbelVanScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState(), true, true);
        var command = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand(VCode: scenario.VCode);

        var sut = new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler(repositoryMock);

        await sut.Handle(command, CancellationToken.None);

        repositoryMock.ShouldHaveSaved(
            WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt.With(scenario.VCode, VerenigingStatus.Actief));
    }
}
