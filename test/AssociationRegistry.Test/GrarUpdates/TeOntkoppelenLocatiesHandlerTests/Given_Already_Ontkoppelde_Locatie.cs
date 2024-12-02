namespace AssociationRegistry.Test.GrarUpdates.TeOntkoppelenLocatiesHandlerTests;

using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Grar.GrarUpdates.TeOnkoppelenLocaties;
using Grar.GrarUpdates.TeOntkoppelenLocaties;
using Vereniging;
using Xunit;

public class Given_Already_Ontkoppelde_Locatie
{
    [Fact]
    public async Task Then_The_Locatie_Is_Ontkoppeld()
    {
        var scenario = new AdresWerdOntkoppeldScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var message = new TeOntkoppelenLocatiesMessage(scenario.VCode,
        [
            scenario.LocatieWerdToegevoegd.Locatie.LocatieId,
        ]);

        var sut = new TeOntkoppelenLocatiesHandler(verenigingRepositoryMock);

        await sut.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
