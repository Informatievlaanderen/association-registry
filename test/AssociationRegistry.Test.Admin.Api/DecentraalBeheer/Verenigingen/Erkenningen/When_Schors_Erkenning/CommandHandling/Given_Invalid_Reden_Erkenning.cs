namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Schors_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Invalid_Reden_Erkenning
{
    public static IEnumerable<object[]> InvalidRedenScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var (vzerErkenningId, vzer) = new ErkenningScenarioBuilder(fixture).WithActieveErkenning().BuildForVzer();

            var (vmrErkenningId, vmr) = new ErkenningScenarioBuilder(fixture).WithActieveErkenning().BuildForVmr();

            string?[] redenValues = [null, ""];

            foreach (var reden in redenValues)
            {
                yield return [vzer, vzerErkenningId, reden];
                yield return [vmr, vmrErkenningId, reden];
            }
        }
    }

    [Theory]
    [MemberData(nameof(InvalidRedenScenarios))]
    public async ValueTask Then_Throw_ErkenningRedenSchorsingVerplicht(
        CommandhandlerScenarioBase scenario,
        int erkenningId,
        string? reden
    )
    {
        var ctx = SchorsErkenningContext<CommandhandlerScenarioBase>.Given(
            scenario,
            _ => erkenningId,
            _ =>
                scenario
                    .GetVerenigingState()
                    .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                    .GeregistreerdDoor.OvoCode
        );

        var exception = await Assert.ThrowsAsync<ErkenningRedenSchorsingIsVerplicht>(async () =>
            await ctx.WithCommand(cmd => cmd with { Erkenning = cmd.Erkenning with { RedenSchorsing = reden } })
                .WhenHandled()
        );

        exception.Message.Should().Be(ExceptionMessages.ErkenningRedenSchorsingVerplicht);
    }
}
