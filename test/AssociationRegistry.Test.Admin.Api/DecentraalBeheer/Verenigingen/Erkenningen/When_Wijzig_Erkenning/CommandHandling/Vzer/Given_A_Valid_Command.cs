namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Primitives;
using Xunit;

public class Given_A_Valid_Command
{
    private readonly WijzigErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask With_All_Fields_Then_It_Adds_An_ErkenningWerdGewijzigd_Event()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                command.Erkenning.ErkenningId,
                command.Erkenning.StartDatum.Value,
                command.Erkenning.EindDatum.Value,
                command.Erkenning.Hernieuwingsdatum.Value,
                command.Erkenning.HernieuwingsUrl,
                ErkenningStatus
                   .Bepaal(
                        ErkenningsPeriode.Create(command.Erkenning.StartDatum.Value, command.Erkenning.EindDatum.Value),
                        DateOnly.FromDateTime(DateTime.Today)
                    )
                   .Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }

    [Fact]
    public async ValueTask With_Startdatum_Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Startdatum_From_Command()
    {
        var nieuweStartdatum = _ctx.Scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value.AddDays(-1);

        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Create(nieuweStartdatum),
            EindDatum = NullOrEmpty<DateOnly>.Null,
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
            HernieuwingsUrl = null,
        };
        var command = _ctx.CreateCommand(teWijzigenErkenning: erkenning);

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                command.Erkenning.ErkenningId,
                command.Erkenning.StartDatum.Value,
                _ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum.Value,
                _ctx.Scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value,
                _ctx.Scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                ErkenningStatus
                   .Bepaal(
                        ErkenningsPeriode.Create(command.Erkenning.StartDatum.Value, _ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum.Value),
                        DateOnly.FromDateTime(DateTime.Today)
                    )
                   .Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }

    [Fact]
    public async ValueTask With_Einddatum_Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Einddatum_From_Command()
    {
        var nieuweEinddatum = _ctx.Scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value.AddDays(1);

        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Null,
            EindDatum = NullOrEmpty<DateOnly>.Create(nieuweEinddatum),
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
            HernieuwingsUrl = null,
        };
        var command = _ctx.CreateCommand(teWijzigenErkenning: erkenning);

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                command.Erkenning.ErkenningId,
                _ctx.Scenario.ErkenningWerdGeregistreerd.Startdatum.Value,
                command.Erkenning.EindDatum.Value,
                _ctx.Scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value,
                _ctx.Scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                ErkenningStatus
                   .Bepaal(
                        ErkenningsPeriode.Create(_ctx.Scenario.ErkenningWerdGeregistreerd.Startdatum, command.Erkenning.EindDatum.Value),
                        DateOnly.FromDateTime(DateTime.Today)
                    )
                   .Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }

    [Fact]
    public async ValueTask With_Hernieuwingsdatum_Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Hernieuwingsdatum_From_Command()
    {
        var einddatum = _ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum!.Value;
        var hernieuwingsdatum = einddatum.AddDays(-1);

        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Null,
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(hernieuwingsdatum),
            EindDatum = NullOrEmpty<DateOnly>.Null,
            HernieuwingsUrl = null,
        };
        var command = _ctx.CreateCommand(teWijzigenErkenning: erkenning);

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                command.Erkenning.ErkenningId,
                _ctx.Scenario.ErkenningWerdGeregistreerd.Startdatum.Value,
                einddatum,
                command.Erkenning.Hernieuwingsdatum.Value,
                _ctx.Scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                ErkenningStatus
                   .Bepaal(
                        ErkenningsPeriode.Create(_ctx.Scenario.ErkenningWerdGeregistreerd.Startdatum, _ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum),
                        DateOnly.FromDateTime(DateTime.Today)
                    )
                   .Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }

    [Fact]
    public async ValueTask With_Valid_Scheme_Hernieuwingsurl_Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Hernieuwingsurl_From_Command()
    {
        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Null,
            EindDatum = NullOrEmpty<DateOnly>.Null,
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
        };
        var command = _ctx.CreateCommand(teWijzigenErkenning: erkenning);

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                command.Erkenning.ErkenningId,
                _ctx.Scenario.ErkenningWerdGeregistreerd.Startdatum.Value,
                _ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum.Value,
                _ctx.Scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value,
                command.Erkenning.HernieuwingsUrl,
                ErkenningStatus
                   .Bepaal(
                        ErkenningsPeriode.Create(_ctx.Scenario.ErkenningWerdGeregistreerd.Startdatum, _ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum),
                        DateOnly.FromDateTime(DateTime.Today)
                    )
                   .Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }

    [Fact]
    public async ValueTask With_Empty_Hernieuwingsurl_Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Empty_Hernieuwingsurl()
    {
        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Null,
            EindDatum = NullOrEmpty<DateOnly>.Null,
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
            HernieuwingsUrl = string.Empty,
        };
        var command = _ctx.CreateCommand(teWijzigenErkenning: erkenning);

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                command.Erkenning.ErkenningId,
                _ctx.Scenario.ErkenningWerdGeregistreerd.Startdatum.Value,
                _ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum.Value,
                _ctx.Scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value,
                string.Empty,
                ErkenningStatus
                   .Bepaal(
                        ErkenningsPeriode.Create(_ctx.Scenario.ErkenningWerdGeregistreerd.Startdatum, _ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum),
                        DateOnly.FromDateTime(DateTime.Today)
                    )
                   .Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }
}
