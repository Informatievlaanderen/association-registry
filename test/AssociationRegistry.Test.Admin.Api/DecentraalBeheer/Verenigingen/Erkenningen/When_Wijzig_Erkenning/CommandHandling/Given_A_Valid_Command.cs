namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Primitives;
using Xunit;

public class Given_A_Valid_Command
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
    public async ValueTask With_All_Fields_Then_It_Adds_An_ErkenningWerdGewijzigd_Event(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        await test.WithDefaultErkenningModification().WithInitiator(test.GetInitiatorOvoCode()).WhenHandled();

        var expectedEvent = test.ExpectedErkenningWerdGewijzigd(hernieuwingsUrl: "https://example.org/renewal");
        test.ShouldHaveSavedErkenningWerdGewijzigd(expectedEvent);
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask With_Startdatum_Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Startdatum_From_Command(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var origineleErkenning = scenario
            .Events()
            .OfType<ErkenningWerdGeregistreerd>()
            .Single(e => e.ErkenningId == erkenningId);

        var nieuweStartdatum = origineleErkenning.Hernieuwingsdatum.Value.AddDays(-1);

        await test.WithCommand(cmd =>
                cmd with
                {
                    Erkenning = cmd.Erkenning with
                    {
                        StartDatum = NullOrEmpty<DateOnly>.Create(nieuweStartdatum),
                        EindDatum = NullOrEmpty<DateOnly>.Null,
                        Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
                        HernieuwingsUrl = null,
                    },
                }
            )
            .WithInitiator(test.GetInitiatorOvoCode())
            .WhenHandled();

        var expectedEvent = test.ExpectedErkenningWerdGewijzigd(startdatum: nieuweStartdatum);
        test.ShouldHaveSavedErkenningWerdGewijzigd(expectedEvent);
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask With_Einddatum_Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Einddatum_From_Command(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var origineleErkenning = scenario
            .Events()
            .OfType<ErkenningWerdGeregistreerd>()
            .Single(e => e.ErkenningId == erkenningId);

        var nieuweEinddatum = origineleErkenning.Hernieuwingsdatum.Value.AddDays(1);

        await test.WithCommand(cmd =>
                cmd with
                {
                    Erkenning = cmd.Erkenning with
                    {
                        StartDatum = NullOrEmpty<DateOnly>.Null,
                        EindDatum = NullOrEmpty<DateOnly>.Create(nieuweEinddatum),
                        Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
                        HernieuwingsUrl = null,
                    },
                }
            )
            .WithInitiator(test.GetInitiatorOvoCode())
            .WhenHandled();

        var expectedEvent = test.ExpectedErkenningWerdGewijzigd(einddatum: nieuweEinddatum);
        test.ShouldHaveSavedErkenningWerdGewijzigd(expectedEvent);
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask With_Hernieuwingsdatum_Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Hernieuwingsdatum_From_Command(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var origineleErkenning = scenario
            .Events()
            .OfType<ErkenningWerdGeregistreerd>()
            .Single(e => e.ErkenningId == erkenningId);

        var hernieuwingsdatum =
            origineleErkenning.Hernieuwingsdatum.Value.AddDays(1) < origineleErkenning.Einddatum.Value
                ? origineleErkenning.Hernieuwingsdatum.Value.AddDays(1)
                : origineleErkenning.Hernieuwingsdatum.Value.AddDays(-1);

        await test.WithCommand(cmd =>
                cmd with
                {
                    Erkenning = cmd.Erkenning with
                    {
                        StartDatum = NullOrEmpty<DateOnly>.Null,
                        Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(hernieuwingsdatum),
                        EindDatum = NullOrEmpty<DateOnly>.Null,
                        HernieuwingsUrl = null,
                    },
                }
            )
            .WithInitiator(test.GetInitiatorOvoCode())
            .WhenHandled();

        var expectedEvent = test.ExpectedErkenningWerdGewijzigd(hernieuwingsdatum: hernieuwingsdatum);
        test.ShouldHaveSavedErkenningWerdGewijzigd(expectedEvent);
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask With_Valid_Scheme_Hernieuwingsurl_Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Hernieuwingsurl_From_Command(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        await test.WithDefaultErkenningModification().WithInitiator(test.GetInitiatorOvoCode()).WhenHandled();

        var expectedEvent = test.ExpectedErkenningWerdGewijzigd(hernieuwingsUrl: "https://example.org/renewal");
        test.ShouldHaveSavedErkenningWerdGewijzigd(expectedEvent);
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask With_Empty_Hernieuwingsurl_Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Empty_Hernieuwingsurl(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        await test.WithCommand(cmd =>
                cmd with
                {
                    Erkenning = cmd.Erkenning with
                    {
                        StartDatum = NullOrEmpty<DateOnly>.Null,
                        EindDatum = NullOrEmpty<DateOnly>.Null,
                        Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
                        HernieuwingsUrl = string.Empty,
                    },
                }
            )
            .WithInitiator(test.GetInitiatorOvoCode())
            .WhenHandled();

        var expectedEvent = test.ExpectedErkenningWerdGewijzigd(hernieuwingsUrl: string.Empty);
        test.ShouldHaveSavedErkenningWerdGewijzigd(expectedEvent);
    }
}
