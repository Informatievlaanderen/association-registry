namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_No_TeWijzigen_Values
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
    public async ValueTask With_RedenVanWijziging_Then_It_Throws_MinstensEenTeWijzigenVeldMoetIngevuldZijn(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var test = WijzigErkenningContext<CommandhandlerScenarioBase>.Given(scenario, _ => erkenningId);

        var exception = await Assert.ThrowsAsync<MinstensEenTeWijzigenVeldMoetIngevuldZijn>(async () =>
            await test.WithCommand(cmd =>
                    cmd with
                    {
                        Erkenning = cmd.Erkenning with
                        {
                            StartDatum = NullOrEmpty<DateOnly>.Null,
                            EindDatum = NullOrEmpty<DateOnly>.Null,
                            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
                            HernieuwingsUrl = null,
                        },
                    }
                )
                .WithInitiator(test.GetInitiatorOvoCode())
                .WhenHandled()
        );

        exception.Message.Should().Be(ExceptionMessages.MinstensEenTeWijzigenVeldMoetIngevuldZijn);
    }
}
