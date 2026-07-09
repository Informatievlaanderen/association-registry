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

            var (vzerErkenningId, vzer) = new ErkenningScenarioBuilder(fixture).WithActieveErkenning().BuildForVzer();

            var (vmrErkenningId, vmr) = new ErkenningScenarioBuilder(fixture).WithActieveErkenning().BuildForVmr();

            return new[] { new object[] { vzer, vzerErkenningId }, new object[] { vmr, vmrErkenningId } };
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
