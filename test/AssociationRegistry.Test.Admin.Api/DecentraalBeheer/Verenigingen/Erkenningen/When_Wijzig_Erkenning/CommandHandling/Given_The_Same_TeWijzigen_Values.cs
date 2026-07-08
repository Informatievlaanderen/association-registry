namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Primitives;
using Xunit;

public class Given_The_Same_TeWijzigen_Values
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
    public async ValueTask Then_Should_Not_Have_Any_Saves(CommandhandlerScenarioBase scenario, int erkenningId)
    {
        var test = WijzigErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var origineleErkenning = scenario
            .Events()
            .OfType<ErkenningWerdGeregistreerd>()
            .Single(e => e.ErkenningId == erkenningId);

        await test.WithCommand(cmd =>
                cmd with
                {
                    Erkenning = cmd.Erkenning with
                    {
                        StartDatum = NullOrEmpty<DateOnly>.Create(origineleErkenning.Startdatum.Value),
                        EindDatum = NullOrEmpty<DateOnly>.Create(origineleErkenning.Einddatum.Value),
                        Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(origineleErkenning.Hernieuwingsdatum.Value),
                        HernieuwingsUrl = origineleErkenning.HernieuwingsUrl,
                    },
                }
            )
            .WithInitiator(test.GetInitiatorOvoCode())
            .WhenHandled();

        test.AggregateSessionMock.ShouldNotHaveAnySaves();
    }
}
