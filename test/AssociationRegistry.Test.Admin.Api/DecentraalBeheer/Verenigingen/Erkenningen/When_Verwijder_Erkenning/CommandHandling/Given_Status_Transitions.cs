namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Verwijder_Erkenning.CommandHandling;

using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_Status_Transitions
{
    public static IEnumerable<object[]> ActieveErkenningScenario
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

    [Theory]
    [MemberData(nameof(ActieveErkenningScenario))]
    public async ValueTask From_Actief_To_Verwijderd_Emits_VerenigingWerdNietLangerErkend(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = new VerwijderErkenningContext<CommandhandlerScenarioBase>(
            scenario,
            _ => erkenningId,
            _ =>
                scenario
                    .GetVerenigingState()
                    .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                    .GeregistreerdDoor.OvoCode
        );

        await ctx.Handle(ctx.VerwijderErkenningCommand);

        ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdVerwijderd(erkenningId),
            new VerenigingWerdNietLangerErkend()
        );
    }
}
