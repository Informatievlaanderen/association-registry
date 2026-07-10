namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Reden_Schorsing_Erkenning.CommandHandling;

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Xunit;

public class Given_Erkenning_Geschorst_Met_Dezelfde_Reden
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
    public async ValueTask Then_No_Changes_Applied(CommandhandlerScenarioBase scenario, int erkenningId)
    {
        var huidigeReden = scenario
            .GetVerenigingState()
            .Erkenningen.Single(e => e.ErkenningId == erkenningId)
            .RedenSchorsing;

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
            .WithCommand(cmd => cmd with { Erkenning = cmd.Erkenning with { RedenSchorsing = huidigeReden } })
            .WhenHandled();

        ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
    }
}
