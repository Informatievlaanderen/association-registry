namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Reden_Schorsing_Erkenning.CommandHandling;

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;

public class Given_A_Valid_Command
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var scenario = new ErkenningScenarioBuilder(fixture)
                .GivenActieveErkenning()
                .GivenErkenningWerdGeschorst()
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
    public async ValueTask Then_It_Saves_An_ErkenningRedenVanSchorsingWerdGecorrigeerd_Event_And_OrganisatieBevoegdheidService_Not_Called(
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
                        .GeregistreerdDoor.OvoCode
            )
            .WithCommand(cmd => cmd)
            .WhenHandled();

        ctx.ShouldHaveSaved(
            new ErkenningRedenVanSchorsingWerdGecorrigeerd(
                ctx.Command.Erkenning.ErkenningId,
                ctx.Command.Erkenning.RedenSchorsing
            )
        );
        ctx.OrganisatieBevoegdheidService.VerifyNever();
    }
}
