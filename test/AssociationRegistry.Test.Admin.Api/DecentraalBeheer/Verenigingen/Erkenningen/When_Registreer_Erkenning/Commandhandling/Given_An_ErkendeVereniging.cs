namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling;

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Xunit;

public class Given_An_ErkendeVereniging
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var scenario = new ErkenningScenarioBuilder(fixture)
                .GivenActieveErkenning()
                .GivenVerenigingWerdErkend()
                .Build();

            return new[] { new object[] { scenario.Vzer }, new object[] { scenario.Vmr } };
        }
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask With_Actieve_Erkenning_Then_ErkenningWerdGeregistreerd(CommandhandlerScenarioBase scenario)
    {
        var ctx = await RegistreerErkenningContext<CommandhandlerScenarioBase>
            .Given(scenario)
            .WithActieveErkenning()
            .WhenHandled();

        ctx.ShouldHaveSaved(ctx.ExpectedErkenningWerdGeregistreerd());
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask With_Niet_Actieve_Erkenning_Then_ErkenningWerdGeregistreerd(
        CommandhandlerScenarioBase scenario
    )
    {
        var ctx = await RegistreerErkenningContext<CommandhandlerScenarioBase>
            .Given(scenario)
            .WithNietActieveErkenning()
            .WhenHandled();

        ctx.ShouldHaveSaved(ctx.ExpectedErkenningWerdGeregistreerd());
    }
}
