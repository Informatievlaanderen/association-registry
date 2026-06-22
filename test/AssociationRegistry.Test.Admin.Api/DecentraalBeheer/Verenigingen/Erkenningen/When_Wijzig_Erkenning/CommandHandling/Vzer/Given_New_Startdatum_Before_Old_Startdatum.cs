namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Primitives;
using Xunit;

public class Given_New_Startdatum_Before_Old_Startdatum
{
    private readonly WijzigErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningInAanvraagScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningInAanvraagScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_It_Adds_An_ErkenningWerdGeWijzigd_Event_With_Status_Actief()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var pastWeek = today.AddDays(-7);
        var nextWeek = today.AddDays(7);

        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Create(pastWeek),
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(today),
            EindDatum = NullOrEmpty<DateOnly>.Create(nextWeek),
        };
        var command = _ctx.WijzigErkenningCommand with
        {
            Erkenning = erkenning,
        };

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                command.Erkenning.ErkenningId,
                command.Erkenning.StartDatum.Value,
                command.Erkenning.EindDatum.Value,
                command.Erkenning.Hernieuwingsdatum.Value,
                command.Erkenning.HernieuwingsUrl,
                ErkenningStatus.Actief.Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }
}
