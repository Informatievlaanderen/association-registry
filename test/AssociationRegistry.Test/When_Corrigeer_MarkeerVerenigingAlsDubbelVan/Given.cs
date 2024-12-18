namespace AssociationRegistry.Test.When_Corrigeer_MarkeerVerenigingAlsDubbelVan;

using Acties.CorrigeerMarkeerAlsDubbel;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;

public class Given_A_Dubbele_Vereniging
{
    [Fact]
    public async Task Then_MarkeerVerenigingAlsDubbelVanWerdGecorrigeerd_Event_Is_Saved()
    {
        var scenario = new VerenigingWerdGemarkeerdAlsDubbelVanScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var command = new CorrigeerMarkeerAlsDubbeleVerenigingCommand(VCode: scenario.VCode);

        var sut = new CorrigeerMarkeerAlsDubbeleVerenigingCommandHandler(repositoryMock);

        await sut.Handle(command, CancellationToken.None);

        repositoryMock.ShouldHaveSaved(new MarkeerVerenigingAlsDubbelVanWerdGecorrigeerd(scenario.VCode));
    }
}

