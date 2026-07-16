namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Primitives;
using Xunit;

public class Given_New_Startdatum_Before_Old_Startdatum
{
    public static IEnumerable<object[]> ScenariosWithErkenningId
    {
        get
        {
            var vzer = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningInAanvraagScenario();
            var vmr = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningInAanvraagScenario();
            return new[]
            {
                new object[] { vzer, vzer.ErkenningWerdGeregistreerd.ErkenningId },
                new object[] { vmr, vmr.ErkenningWerdGeregistreerd.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask Then_It_Adds_An_ErkenningWerdGeWijzigd_Event_With_Status_Actief(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var pastWeek = today.AddDays(-7);
        var nextWeek = today.AddDays(7);

        await test.WithCommand(cmd =>
                cmd with
                {
                    Erkenning = cmd.Erkenning with
                    {
                        StartDatum = NullOrEmpty<DateOnly>.Create(pastWeek),
                        Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(today),
                        EindDatum = NullOrEmpty<DateOnly>.Create(nextWeek),
                    },
                }
            )
            .WithInitiator(test.GetInitiatorOvoCode())
            .WhenHandled();

        var expectedEvent = test.ExpectedErkenningWerdGewijzigd(
            startdatum: pastWeek,
            einddatum: nextWeek,
            hernieuwingsdatum: today
        );

        var events = new List<IEvent> { expectedEvent };

        if (expectedEvent.Status == ErkenningStatus.Actief.Value && !scenario.GetVerenigingState().IsErkend)
        {
            events.Add(new VerenigingWerdErkend());
        }

        test.ShouldHaveSaved(events.ToArray());
    }
}
