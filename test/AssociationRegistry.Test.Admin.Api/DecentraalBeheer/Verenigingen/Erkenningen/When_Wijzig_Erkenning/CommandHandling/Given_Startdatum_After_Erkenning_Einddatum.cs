namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_Startdatum_After_Erkenning_Einddatum
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
    public async ValueTask With_TeCorrigeren_Einddatum_Null_Then_It_Throws_StartdatumLigtNaEinddatum(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var origineleErkenning = scenario
            .Events()
            .OfType<ErkenningWerdGeregistreerd>()
            .Single(e => e.ErkenningId == erkenningId);

        var startDatum = NullOrEmpty<DateOnly>.Create(origineleErkenning.Einddatum.Value.AddDays(1));

        var exception = await Assert.ThrowsAsync<StartdatumLigtNaEinddatum>(async () =>
            await test.WithCommand(cmd =>
                    cmd with
                    {
                        Erkenning = cmd.Erkenning with
                        {
                            StartDatum = startDatum,
                            EindDatum = NullOrEmpty<DateOnly>.Null,
                        },
                    }
                )
                .WithInitiator(test.GetInitiatorOvoCode())
                .WhenHandled()
        );

        exception.Message.Should().Be(ExceptionMessages.StartdatumIsAfterEinddatum);
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask With_TeCorrigeren_Einddatum_Then_It_Throws_StartdatumLigtNaEinddatum(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var origineleErkenning = scenario
            .Events()
            .OfType<ErkenningWerdGeregistreerd>()
            .Single(e => e.ErkenningId == erkenningId);

        var startDatum = NullOrEmpty<DateOnly>.Create(origineleErkenning.Einddatum.Value.AddDays(1));
        var eindDatum = NullOrEmpty<DateOnly>.Create(origineleErkenning.Einddatum.Value.AddDays(-1));

        var exception = await Assert.ThrowsAsync<StartdatumLigtNaEinddatum>(async () =>
            await test.WithCommand(cmd =>
                    cmd with
                    {
                        Erkenning = cmd.Erkenning with { StartDatum = startDatum, EindDatum = eindDatum },
                    }
                )
                .WithInitiator(test.GetInitiatorOvoCode())
                .WhenHandled()
        );

        exception.Message.Should().Be(ExceptionMessages.StartdatumIsAfterEinddatum);
    }
}
