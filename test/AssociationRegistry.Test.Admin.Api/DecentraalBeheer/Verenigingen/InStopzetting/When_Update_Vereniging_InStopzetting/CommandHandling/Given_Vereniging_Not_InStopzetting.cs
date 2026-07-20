namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.InStopzetting.When_Update_Vereniging_InStopzetting.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.InStopzetting.Exceptions;
using Builders;
using Common.Scenarios.CommandHandling;
using Events;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Vereniging_Not_InStopzetting
{
    [Fact]
    public async ValueTask With_InStopzetting_True_Then_It_Adds_An_VerenigingWerdInStopzettingGeplaatst_Event()
    {
        var scenario = new VzerScenarioBuilder().Build();

        var ctx = await UpdateVerenigingInStopzettingContext<CommandhandlerScenarioBase>
            .Given(scenario)
            .WithCommand(cmd => cmd with { InStopzetting = true })
            .WithInitiator(WellknownOvoNumbers.VloOvoCode)
            .WhenHandled();

        ctx.ShouldHaveSaved(new VerenigingWerdInStopzettingGeplaatst());
    }

    [Fact]
    public async ValueTask With_InStopzetting_False_Then_Throws_VerenigingNietInStopzetting()
    {
        var scenario = new VzerScenarioBuilder().Build();

        var ctx = UpdateVerenigingInStopzettingContext<CommandhandlerScenarioBase>
            .Given(scenario)
            .WithCommand(cmd => cmd with { InStopzetting = false })
            .WithInitiator(WellknownOvoNumbers.VloOvoCode);

        var exception = await Assert.ThrowsAsync<VerenigingNietInStopzetting>(async () => await ctx.WhenHandled());

        exception.Message.Should().Be(ExceptionMessages.VerenigingNietInStopzetting);
    }
}
