namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Primitives;
using Xunit;

public class Given_Erkenning_Status_Transitions
{
    public static IEnumerable<object[]> InactiefNaarActiefScenario
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

    public static IEnumerable<object[]> ActiefNaarInactiefScenario
    {
        get
        {
            var vzer =
                new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithActieveErkenningEnWerdErkendScenario();
            var vmr = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithActieveErkenningEnWerdErkendScenario();
            return new[]
            {
                new object[] { vzer, vzer.ErkenningWerdGeregistreerd.ErkenningId },
                new object[] { vmr, vmr.ErkenningWerdGeregistreerd.ErkenningId },
            };
        }
    }

    public static IEnumerable<object[]> MeerdereActieveErkenningenScenario
    {
        get
        {
            var vzer =
                new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithMultipleActieveErkenningenEnWerdErkendScenario();
            var vmr =
                new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMultipleActieveErkenningenEnWerdErkendScenario();
            return new[]
            {
                new object[] { vzer, vzer.ActieveErkenningEen.ErkenningId },
                new object[] { vmr, vmr.ActieveErkenningEen.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(InactiefNaarActiefScenario))]
    public async ValueTask From_Inactief_To_Actief_Emits_VerenigingWerdErkend(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);
        var today = DateOnly.FromDateTime(DateTime.Today);

        await test.WithCommand(cmd =>
                cmd with
                {
                    Erkenning = cmd.Erkenning with
                    {
                        StartDatum = NullOrEmpty<DateOnly>.Create(today.AddDays(-5)),
                        EindDatum = NullOrEmpty<DateOnly>.Create(today.AddDays(10)),
                        Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(today.AddDays(5)),
                    },
                }
            )
            .WithInitiator(test.GetInitiatorOvoCode())
            .WhenHandled();

        var expectedEvent = test.ExpectedErkenningWerdGewijzigd(
            startdatum: today.AddDays(-5),
            einddatum: today.AddDays(10),
            hernieuwingsdatum: today.AddDays(5)
        );

        test.ShouldHaveSavedErkenningWerdGewijzigd(expectedEvent);
    }

    [Theory]
    [MemberData(nameof(ActiefNaarInactiefScenario))]
    public async ValueTask From_Actief_To_Inactief_Emits_VerenigingWerdNietLangerErkend(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);
        var today = DateOnly.FromDateTime(DateTime.Today);

        var nieuweEinddatum = today.AddDays(-1);

        await test.WithCommand(cmd =>
                cmd with
                {
                    Erkenning = cmd.Erkenning with
                    {
                        EindDatum = NullOrEmpty<DateOnly>.Create(nieuweEinddatum),
                        Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(nieuweEinddatum.AddDays(-1)),
                    },
                }
            )
            .WithInitiator(test.GetInitiatorOvoCode())
            .WhenHandled();

        var expectedEvent = test.ExpectedErkenningWerdGewijzigd(
            einddatum: nieuweEinddatum,
            hernieuwingsdatum: nieuweEinddatum.AddDays(-1)
        );

        test.ShouldHaveSaved(expectedEvent, new VerenigingWerdNietLangerErkend());
    }

    [Theory]
    [MemberData(nameof(MeerdereActieveErkenningenScenario))]
    public async ValueTask From_Actief_To_Inactief_With_Another_Active_Erkenning_Does_Not_Emit_VerenigingWerdNietLangerErkend(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);
        var today = DateOnly.FromDateTime(DateTime.Today);

        var nieuweEinddatum = today.AddDays(-1);

        await test.WithCommand(cmd =>
                cmd with
                {
                    Erkenning = cmd.Erkenning with
                    {
                        EindDatum = NullOrEmpty<DateOnly>.Create(nieuweEinddatum),
                        Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(nieuweEinddatum.AddDays(-1)),
                    },
                }
            )
            .WithInitiator(test.GetInitiatorOvoCode())
            .WhenHandled();

        var expectedEvent = test.ExpectedErkenningWerdGewijzigd(
            einddatum: nieuweEinddatum,
            hernieuwingsdatum: nieuweEinddatum.AddDays(-1)
        );

        test.ShouldHaveSavedErkenningWerdGewijzigd(expectedEvent);
    }
}
