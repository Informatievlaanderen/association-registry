namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.InStopzetting.When_Update_Vereniging_InStopzetting.CommandHandling;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AutoFixture;
using Builders;
using Xunit;

public class Given_Vereniging_InStopzetting
{
    [Fact]
    public async ValueTask Then_It_Adds_An_VerenigingWerdInStopzettingGeplaatst_Event()
    {
        var scenario = new VzerScenarioBuilder().Build();

        var ctx = await UpdateVerenigingInStopzettingContext<CommandhandlerScenarioBase>
            .Given(scenario)
            .WithCommand(cmd => cmd with { InStopzetting = true })
            .WithInitiator(WellknownOvoNumbers.VloOvoCode)
            .WhenHandled();

        ctx.ShouldHaveSaved(new VerenigingWerdInStopzettingGeplaatst());
    }
}
