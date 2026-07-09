namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;

public class Given_A_Verlopen_Geschorste_Erkenning
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var (vzerErkenningId, vzer) = new ErkenningScenarioBuilder(fixture)
                .WithTeVerlopenErkenning()
                .WithVerlopenErkenning()
                .WithErkenningWerdGeschorst()
                .BuildForVzer();

            var (vmrErkenningId, vmr) = new ErkenningScenarioBuilder(fixture)
                .WithTeVerlopenErkenning()
                .WithVerlopenErkenning()
                .WithErkenningWerdGeschorst()
                .BuildForVmr();

            return new[] { new object[] { vzer, vzerErkenningId }, new object[] { vmr, vmrErkenningId } };
        }
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask Then_An_SchorsingVanErkenningWerdOpgeheven_Event_Is_Saved(
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
                        .GeregistreerdDoor.OvoCode
            )
            .WithCommand(cmd => cmd)
            .WhenHandled();

        ctx.ShouldHaveSaved(new SchorsingVanErkenningWerdOpgeheven(erkenningId, ErkenningStatus.Verlopen.Value));
    }
}
