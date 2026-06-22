namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Primitives;
using Xunit;

public class Given_A_Verlopen_Erkenning
{
    private readonly WijzigErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask With_Startdatum_In_Past_And_Einddatum_In_Future_Then_Status_Actief()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var startdatum = today.AddDays(-10);
        var hernieuwingsdatum = today.AddDays(5);
        var einddatum = hernieuwingsdatum.AddDays(10);

        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Create(startdatum),
            EindDatum = NullOrEmpty<DateOnly>.Create(einddatum),
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(hernieuwingsdatum),
            HernieuwingsUrl = null,
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
                _ctx.Scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                ErkenningStatus.Actief.Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }
}
