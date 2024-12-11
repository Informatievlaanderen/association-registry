namespace AssociationRegistry.Test.WhenVoegDubbelToe;

using Acties.VoegDubbelToe;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;

public class Given_Valid_VoegDubbelToeCommand
{
    [Fact]
    public async Task Then_Throws_InvalidOperationVerenigingKanGeenDubbelWordenVanZichzelf()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var command = fixture.Create<VoegDubbelToeCommand>()
            with
            {
                VCode = scenario.VCode,
            };
        var sut = new VoegDubbelToeCommandHandler(repositoryMock);

         await sut.Handle(command, CancellationToken.None);

         repositoryMock.ShouldHaveSaved(new VerenigingWerdToegevoegdAlsDubbel(scenario.VCode, command.VCodeDubbeleVereniging));
    }
}
