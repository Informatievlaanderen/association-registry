namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.CommandHandling;

using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_Status_Transitions
{
    public static IEnumerable<object[]> TeActiverenErkenningScenario
    {
        get
        {
            var vzer =
                new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithTeActiverenErkenningScenario();
            var vmr = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithTeActiverenErkenningScenario();

            return new[]
            {
                new object[] { vzer, vzer.ErkenningWerdGeregistreerd.ErkenningId },
                new object[] { vmr, vmr.ErkenningWerdGeregistreerd.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(TeActiverenErkenningScenario))]
    public async ValueTask From_InAanvraag_To_Actief_Emits_VerenigingWerdErkend(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = new ActiveerErkenningContext<CommandhandlerScenarioBase>(
            scenario,
            _ => erkenningId,
            _ =>
                scenario
                    .GetVerenigingState()
                    .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                    .GeregistreerdDoor.OvoCode
        );

        await ctx.Handle(ctx.ActiveerErkenningCommand);

        ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGeactiveerd(erkenningId),
            new VerenigingWerdErkend()
        );
    }
}
