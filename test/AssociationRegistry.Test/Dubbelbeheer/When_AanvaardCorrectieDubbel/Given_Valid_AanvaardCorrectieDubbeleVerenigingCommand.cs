namespace AssociationRegistry.Test.Dubbelbeheer.When_AanvaardCorrectieDubbel;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardCorrectieDubbel;
using DecentraalBeheer.Vereniging;
using Xunit;

public class Given_Valid_AanvaardCorrectieDubbeleVerenigingCommand
{
    [Theory]
    [InlineData(VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype.Feitelijke)]
    [InlineData(VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype.MetRechtspersoonlijkheid)]
    public async Task Then_It_Saves_A_VerenigingAanvaarddeCorrectieDubbeleVereniging(VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype verenigingstype)
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new VerenigingAanvaarddeDubbeleVerenigingScenario(verenigingstype);
        var repositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var command = fixture.Create<AanvaardCorrectieDubbeleVerenigingCommand>()
            with
            {
                VCode = scenario.VCode,
                VCodeDubbeleVereniging = VCode.Create(scenario.VerenigingAanvaarddeDubbeleVereniging.VCodeDubbeleVereniging),
            };

        var sut = new AanvaardCorrectieDubbeleVerenigingCommandHandler(repositoryMock);

        await sut.Handle(command, CancellationToken.None);

        repositoryMock.ShouldHaveSavedExact(new VerenigingAanvaarddeCorrectieDubbeleVereniging(scenario.VCode, command.VCodeDubbeleVereniging));
    }
}
