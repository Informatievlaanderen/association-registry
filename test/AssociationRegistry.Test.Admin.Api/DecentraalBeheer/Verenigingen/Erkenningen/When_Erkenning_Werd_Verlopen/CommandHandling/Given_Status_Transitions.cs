namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Erkenning_Werd_Verlopen.CommandHandling;

using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_Status_Transitions
{
    public static IEnumerable<object[]> TeVerlopenErkenningScenario
    {
        get
        {
            var vzer = new ErkendeVzerWithTeVerlopenErkenningScenario();
            var vmr = new ErkendeVmrWithTeVerlopenErkenningScenario();

            return new[]
            {
                new object[] { vzer, vzer.ErkenningWerdGeregistreerdTeVerlopen.ErkenningId },
                new object[] { vmr, vmr.ErkenningWerdGeregistreerdTeVerlopen.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(TeVerlopenErkenningScenario))]
    public async ValueTask From_Actief_To_Verlopen_Emits_VerenigingWerdNietLangerErkend(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = new VerloopErkenningContext<CommandhandlerScenarioBase>(
            scenario,
            _ => erkenningId,
            _ =>
                scenario
                    .GetVerenigingState()
                    .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                    .GeregistreerdDoor.OvoCode
        );

        await ctx.Handle(ctx.VerloopErkenningCommand);

        ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdVerlopen(erkenningId),
            new VerenigingWerdNietLangerErkend()
        );
    }
}
