namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Schors_Erkenning.CommandHandling.VerenigingErkendStatusTests;

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;

public class Given_Actieve_Erkenning_And_Vereniging_Erkend
{
    public static IEnumerable<object[]> ActieveErkenningScenario
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var (vzerErkenningId, vzer) = new ErkenningScenarioBuilder(fixture)
                .WithActieveErkenning()
                .WithVerenigingWerdErkend()
                .BuildForVzer();

            var (vmerErkenningId, vmr) = new ErkenningScenarioBuilder(fixture)
                .WithActieveErkenning()
                .WithVerenigingWerdErkend()
                .BuildForVmr();

            return new[] { new object[] { vzer, vzerErkenningId }, new object[] { vmr, vmerErkenningId } };
        }
    }

    [Theory]
    [MemberData(nameof(ActieveErkenningScenario))]
    public async ValueTask Then_Saves_ErkenningWerdGeschorst_VerenigingWerdNietLangerErkend(
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
                        .GeregistreerdDoor.OvoCode
            )
            .WithCommand(cmd => cmd)
            .WhenHandled();

        ctx.ShouldHaveSaved(
            new ErkenningWerdGeschorst(erkenningId, ctx.Command.Erkenning.RedenSchorsing),
            new VerenigingWerdNietLangerErkend()
        );
    }
}
