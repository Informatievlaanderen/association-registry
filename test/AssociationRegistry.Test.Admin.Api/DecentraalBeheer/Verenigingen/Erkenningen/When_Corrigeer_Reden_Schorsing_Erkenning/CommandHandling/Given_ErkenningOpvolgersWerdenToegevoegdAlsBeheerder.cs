namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Reden_Schorsing_Erkenning.CommandHandling;

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;

public class Given_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var scenario = new ErkenningScenarioBuilder(fixture)
                .WithActieveErkenning()
                .WithErkenningWerdGeschorst()
                .WithErkenningOpvolgersWerdenToegevoegdAlsBeheerder()
                .Build();

            return new[]
            {
                new object[] { scenario.Vzer, scenario.ErkenningId },
                new object[] { scenario.Vmr, scenario.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask Then_Saves_ErkenningRedenVanSchorsingWerdGecorrigeerd(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = await CorrigeerRedenSchorsingErkenningContext<CommandhandlerScenarioBase>
            .Given(
                scenario,
                _ => erkenningId,
                _ =>
                    scenario
                        .GetVerenigingState()
                        .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                        .Beheerders.First()
            )
            .WithCommand(cmd => cmd)
            .WhenHandled();

        ctx.ShouldHaveSaved(
            new ErkenningRedenVanSchorsingWerdGecorrigeerd(
                ctx.Command.Erkenning.ErkenningId,
                ctx.Command.Erkenning.RedenSchorsing
            )
        );
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask Then_OrganisatieBevoegdheidService_Not_Called(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = await CorrigeerRedenSchorsingErkenningContext<CommandhandlerScenarioBase>
            .Given(
                scenario,
                _ => erkenningId,
                _ =>
                    scenario
                        .GetVerenigingState()
                        .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                        .Beheerders.First()
            )
            .WithCommand(cmd => cmd)
            .WhenHandled();

        ctx.OrganisatieBevoegdheidService.VerifyNever();
    }
}
