namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling.VerenigingErkendStatusTests;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;

public class Given_NietActieve_Erkenning
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var (vzerErkenningId, vzer) = new ErkenningScenarioBuilder(fixture)
                .WithInAanvraagErkenning()
                .WithErkenningWerdGeschorst()
                .BuildForVzer();

            var (vmerErkenningId, vmr) = new ErkenningScenarioBuilder(fixture)
                .WithInAanvraagErkenning()
                .WithErkenningWerdGeschorst()
                .BuildForVmr();

            return new[] { new object[] { vzer, vzerErkenningId }, new object[] { vmr, vmerErkenningId } };
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
                        .GeregistreerdDoor.OvoCode
            )
            .WithCommand(cmd => cmd)
            .WhenHandled();

        ctx.ShouldHaveSaved(new SchorsingVanErkenningWerdOpgeheven(erkenningId, ErkenningStatus.InAanvraag.Value));
    }
}
