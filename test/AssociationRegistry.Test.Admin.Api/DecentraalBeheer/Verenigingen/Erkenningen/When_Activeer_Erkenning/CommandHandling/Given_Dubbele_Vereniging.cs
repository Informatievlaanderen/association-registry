namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.CommandHandling;

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_Dubbele_Vereniging
{
    [Fact]
    public async ValueTask Then_It_Adds_An_ErkenningWerdGeactiveerd_Event()
    {
        var fixture = new Fixture().CustomizeDomain();

        var (erkenningId, scenario) = new ErkenningScenarioBuilder(fixture)
            .WithActieveErkenning()
            .WithVerenigingWerdErkend()
            .WithTeActiverenErkenning()
            .BuildForVzer();

        scenario.additionalEvents.Add(new VerenigingWerdGemarkeerdAlsDubbelVan(scenario.VCode.Value, "V0001001"));

        var ctx = await ActiveerErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario>
            .Given(
                scenario,
                _ => erkenningId,
                _ =>
                    scenario
                        .GetVerenigingState()
                        .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                        .GeregistreerdDoor.OvoCode
            )
            .WithCommand(cmd => cmd)
            .WhenHandled();

        ctx.ShouldHaveSaved(new ErkenningWerdGeactiveerd(erkenningId));
    }
}
