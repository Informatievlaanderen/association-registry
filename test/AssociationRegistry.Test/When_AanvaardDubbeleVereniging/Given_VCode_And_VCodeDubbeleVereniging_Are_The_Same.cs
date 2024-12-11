namespace AssociationRegistry.Test.When_AanvaardDubbeleVereniging;

using AssociationRegistry.Acties.AanvaardDubbel;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Resources;
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
        var command = fixture.Create<AanvaardDubbeleVerenigingCommand>() with
        {
            VCode = scenario.VCode,
            VCodeDubbeleVereniging = scenario.VCode,
        };
        var sut = new AanvaardDubbeleVerenigingCommandHandler(repositoryMock);

        var exception = await Assert.ThrowsAsync<InvalidOperationVerenigingKanGeenDubbelWordenVanZichzelf>(
            async () => await sut.Handle(command, CancellationToken.None));
        exception.Message.Should().Be(ExceptionMessages.VerenigingKanGeenDubbelWordenVanZichzelf);
    }
}
