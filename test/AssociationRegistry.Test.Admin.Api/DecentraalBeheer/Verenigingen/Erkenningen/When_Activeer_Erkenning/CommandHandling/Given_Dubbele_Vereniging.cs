namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.CommandHandling;

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;

public class Given_Dubbele_Vereniging
{
    [Fact]
    public async ValueTask Then_It_Adds_An_ErkenningWerdGeactiveerd_Event()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new ErkenningScenarioBuilder(fixture)
            .GivenActieveErkenning()
            .GivenVerenigingWerdErkend()
            .GivenTeActiverenErkenning()
            .Build();

        scenario.VzerWerdGeregistreerdScenario.additionalEvents.Add(
            new VerenigingWerdGemarkeerdAlsDubbelVan(scenario.VzerWerdGeregistreerdScenario.VCode.Value, "V0001001")
        );

        var ctx = await ActiveerErkenningContext<CommandhandlerScenarioBase>
            .Given(
                scenario.Vzer,
                _ => scenario.ErkenningId,
                _ =>
                    scenario
                        .Vzer.GetVerenigingState()
                        .Erkenningen.Single(e => e.ErkenningId == scenario.ErkenningId)
                        .GeregistreerdDoor.OvoCode
            )
            .WithCommand(cmd => cmd)
            .WhenHandled();

        ctx.ShouldHaveSaved(new ErkenningWerdGeactiveerd(scenario.ErkenningId));
    }
}
