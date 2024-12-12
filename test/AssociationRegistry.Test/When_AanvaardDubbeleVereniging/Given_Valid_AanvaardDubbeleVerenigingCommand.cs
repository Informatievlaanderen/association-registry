namespace AssociationRegistry.Test.When_AanvaardDubbeleVereniging;

using AssociationRegistry.Acties.AanvaardDubbel;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AutoFixture;
using Xunit;

public class Given_Valid_AanvaardDubbeleVerenigingCommand
{
    [Fact]
    public async Task Then_Throws_InvalidOperationVerenigingKanGeenDubbelWordenVanZichzelf()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var command = fixture.Create<AanvaardDubbeleVerenigingCommand>()
            with
            {
                VCode = scenario.VCode,
            };
        var sut = new AanvaardDubbeleVerenigingCommandHandler(repositoryMock);

         await sut.Handle(command, CancellationToken.None);

         repositoryMock.ShouldHaveSaved(new VerenigingAanvaarddeDubbeleVereniging(scenario.VCode, command.VCodeDubbeleVereniging));
    }
}
