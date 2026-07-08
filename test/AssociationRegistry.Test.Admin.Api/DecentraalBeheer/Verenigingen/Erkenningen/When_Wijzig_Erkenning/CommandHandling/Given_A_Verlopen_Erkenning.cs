namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Primitives;
using Xunit;

public class Given_A_Verlopen_Erkenning
{
    public static IEnumerable<object[]> ScenariosWithErkenningId
    {
        get
        {
            var vzer = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario();
            var vmr = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario();
            return new[]
            {
                new object[] { vzer, vzer.ErkenningWerdGeregistreerd.ErkenningId },
                new object[] { vmr, vmr.ErkenningWerdGeregistreerd.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask With_Startdatum_In_Past_And_Einddatum_In_Future_Then_Status_Actief(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var today = DateOnly.FromDateTime(DateTime.Now);
        var startdatum = today.AddDays(-10);
        var hernieuwingsdatum = today.AddDays(5);
        var einddatum = hernieuwingsdatum.AddDays(10);

        await test.WithCommand(cmd =>
                cmd with
                {
                    Erkenning = cmd.Erkenning with
                    {
                        StartDatum = NullOrEmpty<DateOnly>.Create(startdatum),
                        EindDatum = NullOrEmpty<DateOnly>.Create(einddatum),
                        Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(hernieuwingsdatum),
                        HernieuwingsUrl = null,
                    },
                }
            )
            .WithInitiator(test.GetInitiatorOvoCode())
            .WhenHandled();

        var expectedEvent = test.ExpectedErkenningWerdGewijzigd(
            startdatum: startdatum,
            einddatum: einddatum,
            hernieuwingsdatum: hernieuwingsdatum
        );

        var events = new List<IEvent> { expectedEvent };

        if (expectedEvent.Status == ErkenningStatus.Actief.Value && !scenario.GetVerenigingState().IsErkend)
        {
            events.Add(new VerenigingWerdErkend());
        }

        test.ShouldHaveSaved(events.ToArray());
    }
}
