namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.InStopzetting.When_Update_Vereniging_InStopzetting.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.InStopzetting.Exceptions;
using Builders;
using Common.Scenarios.CommandHandling;
using Events;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_A_Stopgezette_Vereniging
{
    [Fact]
    public async ValueTask With_InStopzetting_True_Then_It_Throws_VerenigingNietActiefVoorInStopzetting()
    {
        var scenario = new VzerScenarioBuilder().GivenGestopteVereniging().Build();

        var ctx = UpdateVerenigingInStopzettingContext<CommandhandlerScenarioBase>
            .Given(scenario)
            .WithInitiator(WellknownOvoNumbers.VloOvoCode)
            .WithCommand(cmd => cmd with { InStopzetting = true });

        var exception = await Assert.ThrowsAsync<VerenigingNietActiefVoorInStopzetting>(async () =>
            await ctx.WhenHandled()
        );

        exception.Message.Should().Be(ExceptionMessages.VerenigingNietActiefVoorInStopzetting);
    }
}
