namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_No_RedenVanWijziging
{
    public static IEnumerable<object[]> ScenariosAndRedenVanWijziging
    {
        get
        {
            var vzer = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
            var vmr = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
            var erkenningIdVzer = vzer.ErkenningWerdGeregistreerd.ErkenningId;
            var erkenningIdVmr = vmr.ErkenningWerdGeregistreerd.ErkenningId;

            var redenenVanWijziging = new[] { "", " ", null };

            foreach (var reden in redenenVanWijziging)
            {
                yield return new object[] { vzer, erkenningIdVzer, reden };
                yield return new object[] { vmr, erkenningIdVmr, reden };
            }
        }
    }

    [Theory]
    [MemberData(nameof(ScenariosAndRedenVanWijziging))]
    public async ValueTask Then_It_Throws_RedenVanWijzigingIsVerplicht(
        CommandhandlerScenarioBase scenario,
        int erkenningId,
        string redenVanWijziging
    )
    {
        var test = WijzigErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var exception = await Assert.ThrowsAsync<RedenVanWijzigingIsVerplicht>(async () =>
            await test.WithCommand(cmd =>
                    cmd with
                    {
                        Erkenning = cmd.Erkenning with
                        {
                            StartDatum = NullOrEmpty<DateOnly>.Null,
                            EindDatum = NullOrEmpty<DateOnly>.Null,
                            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
                            HernieuwingsUrl = string.Empty,
                            RedenVanWijziging = redenVanWijziging,
                        },
                    }
                )
                .WithInitiator(test.GetInitiatorOvoCode())
                .WhenHandled()
        );

        exception.Message.Should().Be(ExceptionMessages.RedenVanWijzigingIsVerplicht);
    }
}
