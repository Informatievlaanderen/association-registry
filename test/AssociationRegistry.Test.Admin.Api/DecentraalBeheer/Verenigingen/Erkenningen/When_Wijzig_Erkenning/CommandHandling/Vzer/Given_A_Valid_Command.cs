namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.
    CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Primitives;
using Xunit;

public class Given_A_Valid_Command
{
    private readonly WijzigErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_A_Valid_Command()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new WijzigErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask With_All_Fields_Then_It_Adds_An_ErkenningWerdGewijzigd_Event()
    {
        var teWijzigenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with { ErkenningId = teWijzigenErkenningId },
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        await _commandHandler.Handle(new CommandEnvelope<WijzigErkenningCommand>(command, commandMetadata));

        _verenigingRepositoryMock.ShouldHaveSavedExact(
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
        var teWijzigenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;
        var nieuweStartdatum =
            _scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value.AddDays(-_fixture.Create<int>());

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with
            {
                ErkenningId = teWijzigenErkenningId,
                StartDatum = NullOrEmpty<DateOnly>.Create(nieuweStartdatum),
                EindDatum = NullOrEmpty<DateOnly>.Null,
                Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
                HernieuwingsUrl = null,
            },
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        await _commandHandler.Handle(new CommandEnvelope<WijzigErkenningCommand>(command, commandMetadata));

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                command.Erkenning.ErkenningId,
                command.Erkenning.StartDatum.Value,
                _scenario.ErkenningWerdGeregistreerd.Einddatum.Value,
                _scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value,
                _scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                ErkenningStatus
                   .Bepaal(
                        ErkenningsPeriode.Create(command.Erkenning.StartDatum.Value, _scenario.ErkenningWerdGeregistreerd.Einddatum.Value),
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
        var teWijzigenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var nieuweEinddatum =
            _scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value.AddDays(_fixture.Create<int>());

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with
            {
                ErkenningId = teWijzigenErkenningId,
                StartDatum = NullOrEmpty<DateOnly>.Null,
                EindDatum = NullOrEmpty<DateOnly>.Create(nieuweEinddatum),
                Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
                HernieuwingsUrl = null,
            },
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        await _commandHandler.Handle(new CommandEnvelope<WijzigErkenningCommand>(command, commandMetadata));

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                command.Erkenning.ErkenningId,
                _scenario.ErkenningWerdGeregistreerd.Startdatum.Value,
                command.Erkenning.EindDatum.Value,
                _scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value,
                _scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                ErkenningStatus
                   .Bepaal(
                        ErkenningsPeriode.Create(_scenario.ErkenningWerdGeregistreerd.Startdatum,
                                                 command.Erkenning.EindDatum.Value),
                        DateOnly.FromDateTime(DateTime.Today)
                    )
                   .Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }

    [Fact]
    public async ValueTask
        With_Hernieuwingsdatum_Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Hernieuwingsdatum_From_Command()
    {
        var teWijzigenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;
        var startdatum = _scenario.ErkenningWerdGeregistreerd.Startdatum!.Value;
        var einddatum = _scenario.ErkenningWerdGeregistreerd.Einddatum!.Value;
        var hernieuwingsdatum = CreateHernieuwingsdatumBinnenErkenningsperiode(einddatum, startdatum);

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with
            {
                ErkenningId = teWijzigenErkenningId,
                StartDatum = NullOrEmpty<DateOnly>.Null,
                Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(hernieuwingsdatum),
                EindDatum = NullOrEmpty<DateOnly>.Null,
                HernieuwingsUrl = null,
            },
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        await _commandHandler.Handle(new CommandEnvelope<WijzigErkenningCommand>(command, commandMetadata));

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                command.Erkenning.ErkenningId,
                _scenario.ErkenningWerdGeregistreerd.Startdatum.Value,
                _scenario.ErkenningWerdGeregistreerd.Einddatum.Value,
                command.Erkenning.Hernieuwingsdatum.Value,
                _scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                ErkenningStatus
                   .Bepaal(
                        ErkenningsPeriode.Create(_scenario.ErkenningWerdGeregistreerd.Startdatum,
                                                 _scenario.ErkenningWerdGeregistreerd.Einddatum),
                        DateOnly.FromDateTime(DateTime.Today)
                    )
                   .Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }

    private DateOnly CreateHernieuwingsdatumBinnenErkenningsperiode(DateOnly einddatum, DateOnly startdatum)
    {
        var periodeInDagen = einddatum.DayNumber - startdatum.DayNumber;
        var daysBeforeEinddatum = _fixture.Create<int>() % (periodeInDagen + 1);

        var hernieuwingsdatum = einddatum.AddDays(-daysBeforeEinddatum);

        return hernieuwingsdatum;
    }

    [Fact]
    public async ValueTask
        With_Valid_Scheme_Hernieuwingsurl_Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Hernieuwingsurl_From_Command()
    {
        var teWijzigenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with
            {
                ErkenningId = teWijzigenErkenningId,
                StartDatum = NullOrEmpty<DateOnly>.Null,
                EindDatum = NullOrEmpty<DateOnly>.Null,
                Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
            },
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        await _commandHandler.Handle(new CommandEnvelope<WijzigErkenningCommand>(command, commandMetadata));

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                command.Erkenning.ErkenningId,
                _scenario.ErkenningWerdGeregistreerd.Startdatum.Value,
                _scenario.ErkenningWerdGeregistreerd.Einddatum.Value,
                _scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value,
                command.Erkenning.HernieuwingsUrl,
                ErkenningStatus
                   .Bepaal(
                        ErkenningsPeriode.Create(_scenario.ErkenningWerdGeregistreerd.Startdatum,
                                                 _scenario.ErkenningWerdGeregistreerd.Einddatum),
                        DateOnly.FromDateTime(DateTime.Today)
                    )
                   .Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }

    [Fact]
    public async ValueTask
        With_Empty_Hernieuwingsurl_Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Empty_Hernieuwingsurl()
    {
        var teWijzigenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with
            {
                ErkenningId = teWijzigenErkenningId,
                StartDatum = NullOrEmpty<DateOnly>.Null,
                EindDatum = NullOrEmpty<DateOnly>.Null,
                Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
                HernieuwingsUrl = string.Empty,
            },
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        await _commandHandler.Handle(new CommandEnvelope<WijzigErkenningCommand>(command, commandMetadata));

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                command.Erkenning.ErkenningId,
                _scenario.ErkenningWerdGeregistreerd.Startdatum.Value,
                _scenario.ErkenningWerdGeregistreerd.Einddatum.Value,
                _scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value,
                string.Empty,
                ErkenningStatus
                   .Bepaal(
                        ErkenningsPeriode.Create(_scenario.ErkenningWerdGeregistreerd.Startdatum,
                                                 _scenario.ErkenningWerdGeregistreerd.Einddatum),
                        DateOnly.FromDateTime(DateTime.Today)
                    )
                   .Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }
}
