namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Schors_Erkenning.CommandHandling.VerenigingErkendStatusTests;

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;

public class Given_NietActieve_Erkenning
{
    public static IEnumerable<object[]> NietActieveErkenningScenario
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var scenario = new ErkenningScenarioBuilder(fixture).GivenInAanvraagErkenning().Build();

            return new[]
            {
                new object[] { scenario.Vzer, scenario.ErkenningId },
                new object[] { scenario.Vmr, scenario.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(NietActieveErkenningScenario))]
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
                        .GeregistreerdDoor.OvoCode
            )
            .WithCommand(cmd => cmd)
            .WhenHandled();

        ctx.ShouldHaveSaved(new ErkenningWerdGeschorst(erkenningId, ctx.Command.Erkenning.RedenSchorsing));
    }
}
