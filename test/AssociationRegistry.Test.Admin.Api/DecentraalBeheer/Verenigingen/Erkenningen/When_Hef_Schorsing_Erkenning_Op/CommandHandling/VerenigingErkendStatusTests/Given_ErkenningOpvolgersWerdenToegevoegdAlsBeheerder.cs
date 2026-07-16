namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling.VerenigingErkendStatusTests;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
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
                .GivenActieveErkenning()
                .GivenErkenningWerdGeschorst()
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
    public async ValueTask Then_Saves_SchorsingVanErkenningWerdOpgeheven(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = await HefSchorsingErkenningOpContext<CommandhandlerScenarioBase>
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
            new SchorsingVanErkenningWerdOpgeheven(erkenningId, ErkenningStatus.Actief.Value),
            new VerenigingWerdErkend()
        );
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask Then_OrganisatieBevoegdheidService_Not_Called(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = await HefSchorsingErkenningOpContext<CommandhandlerScenarioBase>
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
