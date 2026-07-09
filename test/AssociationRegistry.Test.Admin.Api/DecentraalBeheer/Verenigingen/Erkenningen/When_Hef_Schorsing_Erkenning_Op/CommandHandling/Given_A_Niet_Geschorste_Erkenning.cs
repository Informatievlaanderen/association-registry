namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_A_Niet_Geschorste_Erkenning
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
    public async ValueTask Then_Throws_ErkenningIsNietGeschorst(CommandhandlerScenarioBase scenario, int erkenningId)
    {
        var ctx = HefSchorsingErkenningOpContext<CommandhandlerScenarioBase>.Given(
            scenario,
            _ => erkenningId,
            _ =>
                scenario
                    .GetVerenigingState()
                    .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                    .GeregistreerdDoor.OvoCode
        );

        var exception = await Assert.ThrowsAsync<ErkenningIsNietGeschorst>(async () =>
            await ctx.WithCommand(cmd => cmd).WhenHandled()
        );

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningIsNietGeschorst));
    }
}
