namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Verleng_Erkenning.
    CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerlengErkenning;
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
    private readonly VerlengErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_A_Valid_Command()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new VerlengErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Saves_An_ErkenningWerdVerlengd_Event()
    {
        var teVerlengenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var status = ErkenningStatus.Bepaal(
            ErkenningsPeriode.Create(
                _scenario.ErkenningWerdGeregistreerd.Startdatum,
                _scenario.ErkenningWerdGeregistreerd.Einddatum
            ),
            DateOnly.FromDateTime(DateTime.Now)
        );

        var hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(_scenario.ErkenningWerdGeregistreerd.Startdatum.Value);

        var commandEnvelope =
            CreateCommandEnvelopeWithTeVerlengenEinddatumAfterCurrentEinddatum(
                teVerlengenErkenningId,
                hernieuwingsdatum);

        await _commandHandler.Handle(commandEnvelope);

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new ErkenningWerdVerlengd(commandEnvelope.Command.Erkenning.ErkenningId,
                                      commandEnvelope.Command.Erkenning.Einddatum,
                                      commandEnvelope.Command.Erkenning.Hernieuwingsdatum.Value,
                                      status.Value
            )
        );
    }

    [Fact]
    public async ValueTask With_Empty_Hernieuwingsdatum_Then_Saves_An_ErkenningWerdVerlengd_Event()
    {
        var erkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var commandEnvelope =
            CreateCommandEnvelopeWithTeVerlengenEinddatumAfterCurrentEinddatum(
                erkenningId,
                NullOrEmpty<DateOnly>.Empty);

        await _commandHandler.Handle(commandEnvelope);

        var newStatus = ErkenningStatus
                       .Bepaal(
                            ErkenningsPeriode.Create(_scenario.ErkenningWerdGeregistreerd.Startdatum.Value,
                                                     commandEnvelope.Command.Erkenning.Einddatum),
                            DateOnly.FromDateTime(DateTime.Today)
                        )
                       .Value;

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new ErkenningWerdVerlengd(commandEnvelope.Command.Erkenning.ErkenningId,
                                      commandEnvelope.Command.Erkenning.Einddatum,
                                      _scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum,
                                      newStatus
            )
        );
    }

    [Fact]
    public async ValueTask With_Null_Hernieuwingsdatum_Then_Saves_An_ErkenningWerdVerlengd_Event()
    {
        var erkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var commandEnvelope =
            CreateCommandEnvelopeWithTeVerlengenEinddatumAfterCurrentEinddatum(
                erkenningId,
                NullOrEmpty<DateOnly>.Null);

        await _commandHandler.Handle(commandEnvelope);

        var newStatus = ErkenningStatus
                       .Bepaal(
                            ErkenningsPeriode.Create(_scenario.ErkenningWerdGeregistreerd.Startdatum.Value,
                                                     commandEnvelope.Command.Erkenning.Einddatum),
                            DateOnly.FromDateTime(DateTime.Today)
                        )
                       .Value;

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new ErkenningWerdVerlengd(commandEnvelope.Command.Erkenning.ErkenningId,
                                      commandEnvelope.Command.Erkenning.Einddatum,
                                      _scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum,
                                      newStatus
            )
        );
    }

    private CommandEnvelope<VerlengErkenningCommand> CreateCommandEnvelopeWithTeVerlengenEinddatumAfterCurrentEinddatum(
        int teVerlengenErkenningId,
        NullOrEmpty<DateOnly> hernieuwingsdatum)
    {
        var command = _fixture.Create<VerlengErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeVerlengenErkenning>() with
            {
                ErkenningId = teVerlengenErkenningId,
                Einddatum = _scenario.ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(_fixture.Create<int>()),
                Hernieuwingsdatum = hernieuwingsdatum,
            },
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        return new CommandEnvelope<VerlengErkenningCommand>(command, commandMetadata);
    }
}
