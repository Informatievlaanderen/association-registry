namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Xunit;

public class Given_Erkenning_InAanvraag
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var scenario = new ErkenningScenarioBuilder(fixture).WithInAanvraagErkenning().Build();

            return new[]
            {
                new object[] { scenario.Vzer, scenario.ErkenningId },
                new object[] { scenario.Vmr, scenario.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask Then_Throws_ErkenningKanNietGeactiveerdWorden(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = ActiveerErkenningContext<CommandhandlerScenarioBase>.Given(
            scenario,
            _ => erkenningId,
            _ =>
                scenario
                    .GetVerenigingState()
                    .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                    .GeregistreerdDoor.OvoCode
        );

        var erkenning = scenario.GetVerenigingState().Erkenningen.Single(e => e.ErkenningId == erkenningId);

        var exception = await Assert.ThrowsAsync<ErkenningKanNietGeactiveerdWorden>(async () =>
            await ctx.WithCommand(cmd => cmd).WhenHandled()
        );

        ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
        exception
            .Message.Should()
            .Be(
                string.Format(
                    "Erkenning met id: {0}, startdatum: {1}, einddatum: {2} en status: {3} kan niet geactiveerd worden.",
                    erkenningId,
                    erkenning.ErkenningsPeriode.Startdatum,
                    erkenning.ErkenningsPeriode.Einddatum,
                    erkenning.Status.Value
                )
            );
    }
}
