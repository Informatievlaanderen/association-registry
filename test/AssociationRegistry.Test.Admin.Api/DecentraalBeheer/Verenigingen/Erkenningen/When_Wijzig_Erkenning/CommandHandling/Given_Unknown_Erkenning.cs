namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Unknown_Erkenning
{
    public static IEnumerable<object[]> ScenariosWithErkenningId
    {
        get
        {
            var vzer = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
            var vmr = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
            return new[]
            {
                new object[] { vzer, vzer.ErkenningWerdGeregistreerd.ErkenningId },
                new object[] { vmr, vmr.ErkenningWerdGeregistreerd.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask Then_Throws_ErkenningIsNietGekend(CommandhandlerScenarioBase scenario, int erkenningId)
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var unknownErkenningId = erkenningId + 999;

        var exception = await Assert.ThrowsAsync<ErkenningIsNietGekend>(async () =>
            await test.WithDefaultErkenningModification()
                .WithInitiator(test.GetInitiatorOvoCode())
                .WithCommand(cmd => cmd with { Erkenning = cmd.Erkenning with { ErkenningId = unknownErkenningId } })
                .WhenHandled()
        );

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningIsNietGekend, unknownErkenningId));
    }
}
