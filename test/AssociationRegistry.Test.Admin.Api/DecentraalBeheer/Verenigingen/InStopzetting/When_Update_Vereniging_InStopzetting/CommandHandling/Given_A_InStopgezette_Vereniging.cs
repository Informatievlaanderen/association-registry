namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.InStopzetting.When_Update_Vereniging_InStopzetting.CommandHandling;

using Builders;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;

public class Given_A_InStopgezette_Vereniging
{
    [Fact]
    public async ValueTask With_InStopzetting_False_Then_It_Saves_VerenigingWerdUitStopzettingGehaald()
    {
        var scenario = new VzerScenarioBuilder().GivenInStopgezetteVereniging().Build();

        var ctx = await UpdateVerenigingInStopzettingContext<CommandhandlerScenarioBase>
            .Given(scenario)
            .WithInitiator(WellknownOvoNumbers.VloOvoCode)
            .WithCommand(cmd => cmd with { InStopzetting = false })
            .WhenHandled();

        ctx.ShouldHaveSaved(new VerenigingWerdUitStopzettingGehaald());
    }
}
