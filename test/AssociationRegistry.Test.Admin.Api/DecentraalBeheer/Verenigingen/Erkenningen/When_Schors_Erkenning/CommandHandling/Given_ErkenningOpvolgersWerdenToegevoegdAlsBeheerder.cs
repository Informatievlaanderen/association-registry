namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Schors_Erkenning.CommandHandling;

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

            var (vzerErkenningId, vzer) = new ErkenningScenarioBuilder(fixture)
                .WithActieveErkenning()
                .WithErkenningOpvolgersWerdenToegevoegdAlsBeheerder()
                .BuildForVzer();

            var (vmrErkenningId, vmr) = new ErkenningScenarioBuilder(fixture)
                .WithActieveErkenning()
                .WithErkenningOpvolgersWerdenToegevoegdAlsBeheerder()
                .BuildForVmr();

            return new[] { new object[] { vzer, vzerErkenningId }, new object[] { vmr, vmrErkenningId } };
        }
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask Then_Saves_ErkenningWerdGeschorst(CommandhandlerScenarioBase scenario, int erkenningId)
    {
        var ctx = await SchorsErkenningContext<CommandhandlerScenarioBase>
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
            new ErkenningWerdGeschorst(ctx.Command.Erkenning.ErkenningId, ctx.Command.Erkenning.RedenSchorsing)
        );
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask Then_OrganisatieBevoegdheidService_Not_Called(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = await SchorsErkenningContext<CommandhandlerScenarioBase>
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
