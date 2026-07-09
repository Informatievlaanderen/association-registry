namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Verwijder_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Unknown_Erkenning
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var scenario = new ErkenningScenarioBuilder(fixture).WithActieveErkenning().Build();

            return new[]
            {
                new object[] { scenario.Vzer, scenario.ErkenningId },
                new object[] { scenario.Vmr, scenario.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask Then_Throws_ErkenningIsNietGekend(CommandhandlerScenarioBase scenario, int erkenningId)
    {
        var ctx = VerwijderErkenningContext<CommandhandlerScenarioBase>.Given(
            scenario,
            _ => erkenningId,
            _ =>
                scenario
                    .GetVerenigingState()
                    .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                    .GeregistreerdDoor.OvoCode
        );

        var unknownErkenningId = ctx.CreateUnknownErkenningId();

        var exception = await Assert.ThrowsAsync<ErkenningIsNietGekend>(async () =>
            await ctx.WithCommand(cmd => cmd with { ErkenningId = unknownErkenningId }).WhenHandled()
        );

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningIsNietGekend, unknownErkenningId));
    }
}
