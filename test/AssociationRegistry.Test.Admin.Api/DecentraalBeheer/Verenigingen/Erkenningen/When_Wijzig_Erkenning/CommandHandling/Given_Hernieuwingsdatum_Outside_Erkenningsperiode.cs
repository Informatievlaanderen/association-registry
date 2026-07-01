namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_Hernieuwingsdatum_Outside_Erkenningsperiode
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
    public async ValueTask With_Hernieuwingsdatum_Before_Erkenning_Startdatum_Then_It_Throws_HernieuwingsDatumMoetTussenStartEnEindDatumLiggen(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var origineleErkenning = scenario
            .Events()
            .OfType<ErkenningWerdGeregistreerd>()
            .Single(e => e.ErkenningId == erkenningId);

        var hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(origineleErkenning.Startdatum.Value.AddDays(-1));

        var exception = await Assert.ThrowsAsync<HernieuwingsDatumMoetTussenStartEnEindDatumLiggen>(async () =>
            await test.WithCommand(cmd =>
                    cmd with
                    {
                        Erkenning = cmd.Erkenning with
                        {
                            StartDatum = NullOrEmpty<DateOnly>.Null,
                            EindDatum = NullOrEmpty<DateOnly>.Null,
                            Hernieuwingsdatum = hernieuwingsdatum,
                        },
                    }
                )
                .WithInitiator(test.GetInitiatorOvoCode())
                .WhenHandled()
        );

        exception.Message.Should().Be(ExceptionMessages.HernieuwingsDatumMoetTussenStartEnEindDatumLiggen);
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask With_Hernieuwingsdatum_After_Erkenning_Einddatum_Then_It_Throws_HernieuwingsDatumMoetTussenStartEnEindDatumLiggen(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var origineleErkenning = scenario
            .Events()
            .OfType<ErkenningWerdGeregistreerd>()
            .Single(e => e.ErkenningId == erkenningId);

        var hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(origineleErkenning.Einddatum.Value.AddDays(1));

        var exception = await Assert.ThrowsAsync<HernieuwingsDatumMoetTussenStartEnEindDatumLiggen>(async () =>
            await test.WithCommand(cmd =>
                    cmd with
                    {
                        Erkenning = cmd.Erkenning with
                        {
                            StartDatum = NullOrEmpty<DateOnly>.Null,
                            EindDatum = NullOrEmpty<DateOnly>.Null,
                            Hernieuwingsdatum = hernieuwingsdatum,
                        },
                    }
                )
                .WithInitiator(test.GetInitiatorOvoCode())
                .WhenHandled()
        );

        exception.Message.Should().Be(ExceptionMessages.HernieuwingsDatumMoetTussenStartEnEindDatumLiggen);
    }
}
