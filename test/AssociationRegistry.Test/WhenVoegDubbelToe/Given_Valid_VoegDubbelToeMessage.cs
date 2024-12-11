namespace AssociationRegistry.Test.WhenVoegDubbelToe;

using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Vereniging.Dubbels;
using Xunit;

public class Given_Valid_VoegDubbelToeMessage
{
    [Fact]
    public async Task Then_Throws_InvalidOperationVerenigingKanGeenDubbelWordenVanZichzelf()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var message = fixture.Create<VoegDubbelToeMessage>()
            with
            {
                VCode = scenario.VCode,
            };
        var sut = new VoegDubbelToeMessageHandler(repositoryMock);

         await sut.Handle(message, CancellationToken.None);

         repositoryMock.ShouldHaveSaved(new DubbeleVerenigingWerdToegevoegd(scenario.VCode, message.VCodeDubbeleVereniging));
    }
}
