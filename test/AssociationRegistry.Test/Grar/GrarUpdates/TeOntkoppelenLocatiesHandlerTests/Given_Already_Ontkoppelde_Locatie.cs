namespace AssociationRegistry.Test.Grar.GrarUpdates.TeOntkoppelenLocatiesHandlerTests;

using AssociationRegistry.Grar.GrarConsumer.Messaging.OntkoppelAdres;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Xunit;

public class Given_Already_Ontkoppelde_Locatie
{
    [Fact]
    public async ValueTask Then_The_Locatie_Is_Ontkoppeld()
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
