namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_Status_Transitions
{
    public static IEnumerable<object[]> GeschorsteActieveErkenningScenario
    {
        get
        {
            var vzer =
                new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteActieveErkenningScenario();
            var vmr = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteActieveErkenningScenario();

            return new[]
            {
                new object[] { vzer, vzer.ErkenningWerdGeregistreerd.ErkenningId },
                new object[] { vmr, vmr.ErkenningWerdGeregistreerd.ErkenningId },
            };
        }
    }

    [Theory]
    [MemberData(nameof(GeschorsteActieveErkenningScenario))]
    public async ValueTask From_Geschorst_To_Actief_Emits_VerenigingWerdErkend(
        CommandhandlerScenarioBase scenario,
        int erkenningId
    )
    {
        var ctx = new HefSchorsingErkenningOpContext<CommandhandlerScenarioBase>(
            scenario,
            _ => erkenningId,
            _ =>
                scenario
                    .GetVerenigingState()
                    .Erkenningen.Single(e => e.ErkenningId == erkenningId)
                    .GeregistreerdDoor.OvoCode
        );

        await ctx.Handle(ctx.HefSchorsingErkenningOpCommand);

        var erkenning = scenario.GetVerenigingState().Erkenningen.Single(e => e.ErkenningId == erkenningId);

        var status = ErkenningStatus.Bepaal(
            ErkenningsPeriode.Create(erkenning.ErkenningsPeriode.Startdatum, erkenning.ErkenningsPeriode.Einddatum),
            DateOnly.FromDateTime(DateTime.Now)
        );

        ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new SchorsingVanErkenningWerdOpgeheven(erkenningId, status.Value),
            new VerenigingWerdErkend()
        );
    }
}
