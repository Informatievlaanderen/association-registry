namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Verwijder_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Another_OvoCode
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
    public async ValueTask Then_Throw_GiIsNietBevoegd(CommandhandlerScenarioBase scenario, int erkenningId)
    {
        var ctx = VerwijderErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var exception = await Assert.ThrowsAsync<GiIsNietBevoegd>(async () =>
            await ctx.WithCommand(cmd => cmd).WhenHandled()
        );

        exception.Message.Should().Be(ExceptionMessages.GiIsNietBevoegd);
    }
}
