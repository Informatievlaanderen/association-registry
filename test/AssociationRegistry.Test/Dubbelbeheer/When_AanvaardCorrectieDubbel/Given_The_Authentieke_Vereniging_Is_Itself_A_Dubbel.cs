namespace AssociationRegistry.Test.Dubbelbeheer.When_AanvaardCorrectieDubbel;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardCorrectieDubbel;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using Xunit;

/// <summary>
///     A is dubbel van B en B is later dubbel geworden van C.
///     Wordt de markering van A gecorrigeerd, dan moet B die correctie nog steeds aanvaarden.
/// </summary>
public class Given_The_Authentieke_Vereniging_Is_Itself_A_Dubbel
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

        var state = scenario
            .GetVerenigingState()
            .Apply(new VerenigingWerdGemarkeerdAlsDubbelVan(scenario.VCode, fixture.Create<VCode>()));

        var aggregateSession = new AggregateSessionMock(state, expectedLoadingDubbel: true);

        var command = new AanvaardCorrectieDubbeleVerenigingCommand(
            scenario.VCode,
            VCode.Create(scenario.VerenigingAanvaarddeDubbeleVereniging.VCodeDubbeleVereniging)
        );

        var sut = new AanvaardCorrectieDubbeleVerenigingCommandHandler(aggregateSession);

        await sut.Handle(command, CancellationToken.None);

        aggregateSession.ShouldHaveSavedExact(
            new VerenigingAanvaarddeCorrectieDubbeleVereniging(scenario.VCode, command.VCodeDubbeleVereniging)
        );
    }
}
