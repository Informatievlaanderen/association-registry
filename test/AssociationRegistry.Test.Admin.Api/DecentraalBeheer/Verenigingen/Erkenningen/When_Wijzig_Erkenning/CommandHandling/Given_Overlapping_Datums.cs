namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_Overlapping_Datums
{
    public static IEnumerable<object[]> ScenariosWithErkenningId
    {
        get
        {
            var vzer = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario();
            var vmr = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario();
            return new[]
            {
                new object[] { vzer, vzer.ErkenningWerdGeregistreerdInVerleden.ErkenningId },
                new object[] { vmr, vmr.ErkenningWerdGeregistreerdInVerleden.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(ScenariosWithErkenningId))]
    public async ValueTask Then_Throws_ErkenningBestaatAl(CommandhandlerScenarioBase scenario, int erkenningId)
    {
        var test = WijzigErkenningTest<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var origineleErkenning = scenario
            .Events()
            .OfType<ErkenningWerdGeregistreerd>()
            .Single(e => e.ErkenningId == erkenningId);

        var andereErkenning = scenario
            .Events()
            .OfType<ErkenningWerdGeregistreerd>()
            .Single(e => e.Status == ErkenningStatus.Actief.Value);

        var exception = await Assert.ThrowsAsync<ErkenningCombinatieBestaatAl>(async () =>
            await test.WithCommand(cmd =>
                    cmd with
                    {
                        Erkenning = cmd.Erkenning with
                        {
                            StartDatum = NullOrEmpty<DateOnly>.Create(andereErkenning.Startdatum.Value),
                            EindDatum = NullOrEmpty<DateOnly>.Create(andereErkenning.Einddatum.Value),
                            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(andereErkenning.Hernieuwingsdatum.Value),
                        },
                    }
                )
                .WithInitiator(test.GetInitiatorOvoCode())
                .WhenHandled()
        );

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningBestaatAl, erkenningId));
    }
}
