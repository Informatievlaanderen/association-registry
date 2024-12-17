namespace AssociationRegistry.Test.GrarUpdates.TeOntkoppelenLocatiesHandlerTests;

using Acties.GrarConsumer.OntkoppelAdres;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;
using Vereniging;
using Xunit;

public class Given_Already_Ontkoppelde_Locatie
{
    [Fact]
    public async Task Then_The_Locatie_Is_Ontkoppeld()
    {
        var scenario = new AdresWerdOntkoppeldScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState(), expectedLoadingDubbel: true);

        var message = new OntkoppelLocatiesMessage(scenario.VCode,
        [
            scenario.LocatieWerdToegevoegd.Locatie.LocatieId,
        ]);

        var sut = new OntkoppelLocatiesMessageHandler(verenigingRepositoryMock);

        await sut.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
