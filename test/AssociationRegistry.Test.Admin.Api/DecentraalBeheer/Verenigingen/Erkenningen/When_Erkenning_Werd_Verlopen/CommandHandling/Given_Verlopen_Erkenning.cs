namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Erkenning_Werd_Verlopen.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Xunit;

public class Given_Verlopen_Erkenning
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var scenario = new ErkenningScenarioBuilder(fixture)
                .GivenActieveErkenning()
                .GivenVerlopenErkenning()
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
    public async ValueTask Then_No_Saved_Event(CommandhandlerScenarioBase scenario, int erkenningId)
    {
        var ctx = VerloopErkenningContext<CommandhandlerScenarioBase>.Given(
            scenario,
            _ => erkenningId,
            _ =>
                scenario
                    .GetVerenigingState()
                    .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                    .GeregistreerdDoor.OvoCode
        );

        var erkenning = scenario.GetVerenigingState().Erkenningen.Single(e => e.ErkenningId == erkenningId);

        var exception = await Assert.ThrowsAsync<ErkenningKanNietVerlopenWorden>(async () =>
            await ctx.WithCommand(cmd => cmd).WhenHandled()
        );

        ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
        exception
            .Message.Should()
            .Be(
                string.Format(
                    "Erkenning met id: {0}, startdatum: {1}, einddatum: {2} en status: {3} kan niet verlopen worden.",
                    erkenningId,
                    erkenning.ErkenningsPeriode.Startdatum,
                    erkenning.ErkenningsPeriode.Einddatum,
                    erkenning.Status.Value
                )
            );
    }
}
