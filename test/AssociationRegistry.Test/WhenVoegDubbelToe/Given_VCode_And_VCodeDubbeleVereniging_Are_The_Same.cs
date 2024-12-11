namespace AssociationRegistry.Test.WhenVoegDubbelToe;

using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Dubbels;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;

public class Given_VCode_And_VCodeDubbeleVereniging_Are_The_Same
{
    [Fact]
    public async Task Then_Throws_InvalidOperationVerenigingKanGeenDubbelWordenVanZichzelf()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var vCode = fixture.Create<VCode>();
        var message = fixture.Create<VoegDubbelToeMessage>() with
        {
            VCode = scenario.VCode,
            VCodeDubbeleVereniging = scenario.VCode,
        };
        var sut = new VoegDubbelToeMessageHandler(repositoryMock);

        await Assert.ThrowsAsync<InvalidOperationVerenigingKanGeenDubbelWordenVanZichzelf>(
            async () => await sut.Handle(message, CancellationToken.None));
    }
}
