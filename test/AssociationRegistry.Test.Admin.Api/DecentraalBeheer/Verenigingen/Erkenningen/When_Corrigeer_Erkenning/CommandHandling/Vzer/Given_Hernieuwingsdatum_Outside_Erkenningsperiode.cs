namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Erkenning.
    CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_Hernieuwingsdatum_Outside_Erkenningsperiode
{
    private readonly CorrigeerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Hernieuwingsdatum_Outside_Erkenningsperiode()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new CorrigeerErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask With_Hernieuwingsdatum_Before_Erkenning_Startdatum_Then_It_Throws_HernieuwingsDatumMoetTussenStartEnEindDatumLiggen()
    {
        var teCorrigerenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var hernieuwingsdatum =
            NullOrEmpty<DateOnly>.Create(_scenario.ErkenningWerdGeregistreerd.Startdatum.Value.AddDays(-1));

        var command = _fixture.Create<CorrigeerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeCorrigerenErkenning>() with
            {
                ErkenningId = teCorrigerenErkenningId,
                StartDatum = NullOrEmpty<DateOnly>.Null,
                EindDatum = NullOrEmpty<DateOnly>.Null,
                Hernieuwingsdatum = hernieuwingsdatum,
            },
        };

        var exception = await Assert.ThrowsAsync<HernieuwingsDatumMoetTussenStartEnEindDatumLiggen>(async () =>
        {
            var commandMetadata = _fixture.Create<CommandMetadata>() with
            {
                Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode
            };

            await _commandHandler.Handle(
                new CommandEnvelope<
                    CorrigeerErkenningCommand>(
                    command,
                    commandMetadata)
            );
        });

        exception.Message.Should().Be(ExceptionMessages.HernieuwingsDatumMoetTussenStartEnEindDatumLiggen);
    }

    [Fact]
    public async ValueTask With_Hernieuwingsdatum_After_Erkenning_Einddatum_Then_It_Throws_HernieuwingsDatumMoetTussenStartEnEindDatumLiggen()
    {
        var teCorrigerenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var hernieuwingsdatum =
            NullOrEmpty<DateOnly>.Create(_scenario.ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(1));

        var command = _fixture.Create<CorrigeerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeCorrigerenErkenning>() with
            {
                ErkenningId = teCorrigerenErkenningId,
                StartDatum = NullOrEmpty<DateOnly>.Null,
                EindDatum = NullOrEmpty<DateOnly>.Null,
                Hernieuwingsdatum = hernieuwingsdatum,
            },
        };

        var exception = await Assert.ThrowsAsync<HernieuwingsDatumMoetTussenStartEnEindDatumLiggen>(async () =>
        {
            var commandMetadata = _fixture.Create<CommandMetadata>() with
            {
                Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode
            };

            await _commandHandler.Handle(
                new CommandEnvelope<
                    CorrigeerErkenningCommand>(
                    command,
                    commandMetadata)
            );
        });

        exception.Message.Should().Be(ExceptionMessages.HernieuwingsDatumMoetTussenStartEnEindDatumLiggen);
    }
}
