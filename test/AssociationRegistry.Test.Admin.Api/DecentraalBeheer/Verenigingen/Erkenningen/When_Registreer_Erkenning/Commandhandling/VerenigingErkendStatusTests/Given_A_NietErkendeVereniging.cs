namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling.VerenigingErkendStatusTests;

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Events;
using Xunit;

public class Given_A_NietErkendeVereniging
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var scenario = new ErkenningScenarioBuilder(fixture).Build();

            return new[] { new object[] { scenario.Vzer }, new object[] { scenario.Vmr } };
        }
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask With_Actieve_Erkenning_Then_ErkenningWerdGeregistreerd_And_Vereniging_Werd_Erkend(
        CommandhandlerScenarioBase scenario
    )
    {
        var ctx = await RegistreerErkenningContext<CommandhandlerScenarioBase>
            .Given(scenario)
            .WithActieveErkenning()
            .WhenHandled();

        ctx.ShouldHaveSaved(ctx.ExpectedErkenningWerdGeregistreerd(), new VerenigingWerdErkend());
    }
}
