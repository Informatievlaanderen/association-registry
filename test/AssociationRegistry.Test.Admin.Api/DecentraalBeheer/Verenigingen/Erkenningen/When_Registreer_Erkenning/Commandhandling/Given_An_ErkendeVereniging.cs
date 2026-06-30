namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling.Vzer;

using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Xunit;

public class Given_An_ErkendeVereniging
{
    public static IEnumerable<object[]> ErkendeVerenigingen =>
        new[]
        {
            new object[] { new VerenigingZonderEigenRechtspersoonlijkheidWerdErkendScenario() },
            new object[] { new VerenigingMetRechtspersoonlijkheidWerdErkendScenario() },
        };

    [Theory]
    [MemberData(nameof(ErkendeVerenigingen))]
    public async ValueTask With_Actieve_Erkenning_Then_ErkenningWerdGeregistreerd(CommandhandlerScenarioBase scenario)
    {
        var test = await RegistreerErkenningTest<CommandhandlerScenarioBase>
            .Given(scenario)
            .WithActieveErkenning()
            .WhenHandled();

        test.ShouldHaveSaved(test.ExpectedErkenningWerdGeregistreerd());
    }

    [Theory]
    [MemberData(nameof(ErkendeVerenigingen))]
    public async ValueTask With_Niet_Actieve_Erkenning_Then_ErkenningWerdGeregistreerd(
        CommandhandlerScenarioBase scenario
    )
    {
        var test = await RegistreerErkenningTest<CommandhandlerScenarioBase>
            .Given(scenario)
            .WithNietActieveErkenning()
            .WhenHandled();

        test.ShouldHaveSaved(test.ExpectedErkenningWerdGeregistreerd());
    }
}
