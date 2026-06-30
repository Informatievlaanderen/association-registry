namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling.Vzer;

using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_NietErkendeVereniging
{
    public static IEnumerable<object[]> NietErkendeVerenigingen =>
        new[]
        {
            new object[] { new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario() },
            new object[] { new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario() },
        };

    [Theory]
    [MemberData(nameof(NietErkendeVerenigingen))]
    public async ValueTask With_Actieve_Erkenning_Then_ErkenningWerdGeregistreerd_And_Vereniging_Werd_Erkend(
        CommandhandlerScenarioBase scenarioWithNietErkendeVereniging
    )
    {
        var test = await RegistreerErkenningTest<CommandhandlerScenarioBase>
            .Given(scenarioWithNietErkendeVereniging)
            .WithActieveErkenning()
            .WhenHandled();

        test.ShouldHaveSaved(test.ExpectedErkenningWerdGeregistreerd(), new VerenigingWerdErkend());
    }

    [Theory]
    [MemberData(nameof(NietErkendeVerenigingen))]
    public async ValueTask With_Niet_Actieve_Erkenning_Then_ErkenningWerdGeregistreerd(
        CommandhandlerScenarioBase scenarioWithNietErkendeVereniging
    )
    {
        var test = await RegistreerErkenningTest<CommandhandlerScenarioBase>
            .Given(scenarioWithNietErkendeVereniging)
            .WithNietActieveErkenning()
            .WhenHandled();

        test.ShouldHaveSaved(test.ExpectedErkenningWerdGeregistreerd());
    }
}
