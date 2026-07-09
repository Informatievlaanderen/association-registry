namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Reden_Schorsing_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Invalid_Reden_Erkenning
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var scenario = new ErkenningScenarioBuilder(fixture)
                .WithActieveErkenning()
                .WithErkenningWerdGeschorst()
                .Build();

            return new[]
            {
                new object[] { scenario.Vzer, scenario.ErkenningId, "" },
                new object[] { scenario.Vzer, scenario.ErkenningId, null! },
                new object[] { scenario.Vmr, scenario.ErkenningId, "" },
                new object[] { scenario.Vmr, scenario.ErkenningId, null! },
            };
        }
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask Then_Throw_ErkenningRedenSchorsingVerplicht(
        CommandhandlerScenarioBase scenario,
        int erkenningId,
        string reden
    )
    {
        var ctx = CorrigeerRedenSchorsingErkenningContext<CommandhandlerScenarioBase>.Given(
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

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningRedenSchorsingVerplicht));
    }
}
