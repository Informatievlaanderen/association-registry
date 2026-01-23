namespace AssociationRegistry.Test.Dubbelbeheer.When_AanvaardCorrectieDubbel;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardCorrectieDubbel;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using MartenDb.Store;
using Xunit;

public class Given_Valid_AanvaardCorrectieDubbeleVerenigingCommand_Twice
{
    [Theory]
    [InlineData(VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype.Feitelijke)]
    [InlineData(VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype.MetRechtspersoonlijkheid)]
    public async Task Then_It_Saves_A_VerenigingAanvaarddeCorrectieDubbeleVereniging(
        VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype verenigingstype
    )
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new VerenigingAanvaarddeDubbeleVerenigingScenario(verenigingstype);
        var aggregateSession = new AggregateSessionMock(scenario.GetVerenigingState());

        var command = fixture.Create<AanvaardCorrectieDubbeleVerenigingCommand>() with
        {
            VCode = scenario.VCode,
            VCodeDubbeleVereniging = VCode.Create(
                scenario.VerenigingAanvaarddeDubbeleVereniging.VCodeDubbeleVereniging
            ),
        };

        var sut = new AanvaardCorrectieDubbeleVerenigingCommandHandler(aggregateSession);

        await sut.Handle(command, CancellationToken.None);

        await Assert.ThrowsAsync<ApplicationException>(async () => await sut.Handle(command, CancellationToken.None));
    }
}
