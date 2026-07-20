namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.InStopzetting.When_Update_Vereniging_InStopzetting.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Builders;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_A_Vmer
{
    [Fact]
    public async ValueTask Then_It_Throws_ActieIsNietToegestaanVoorVerenigingstype()
    {
        var scenario = new VmerScenarioBuilder().GivenActieveVereniging().Build();

        var ctx = UpdateVerenigingInStopzettingContext<CommandhandlerScenarioBase>
            .Given(scenario)
            .WithInitiator(WellknownOvoNumbers.VloOvoCode)
            .WithCommand(cmd => cmd with { InStopzetting = true });

        var exception = await Assert.ThrowsAsync<ActieIsNietToegestaanVoorVerenigingstype>(async () =>
            await ctx.WhenHandled()
        );

        exception.Message.Should().Be(ExceptionMessages.UnsupportedOperationForVerenigingstype);
    }
}
