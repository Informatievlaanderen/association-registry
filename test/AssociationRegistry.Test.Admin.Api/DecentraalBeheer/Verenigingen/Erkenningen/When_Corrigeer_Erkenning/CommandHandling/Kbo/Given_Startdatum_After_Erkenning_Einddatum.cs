namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_Startdatum_After_Erkenning_Einddatum
{
    private readonly WijzigErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Startdatum_After_Erkenning_Einddatum()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new WijzigErkenningCommandHandler(_verenigingRepositoryMock);
    }

       [Fact]
    public async ValueTask With_TeCorrigeren_Einddatum_Null_Then_It_Throws_StartdatumLigtNaEinddatum()
    {
        var teSchorsenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var startDatum = NullOrEmpty<DateOnly>.Create(_scenario.ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(1));

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with
            {
                ErkenningId = teSchorsenErkenningId,
                StartDatum = startDatum,
                EindDatum = NullOrEmpty<DateOnly>.Null,
            },
        };

        var exception = await Assert.ThrowsAsync<StartdatumLigtNaEinddatum>(async () =>
        {
            var commandMetadata = _fixture.Create<CommandMetadata>() with
            {
                Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode
            };

            await _commandHandler.Handle(
                new CommandEnvelope<
                    WijzigErkenningCommand>(
                    command,
                    commandMetadata)
            );
        });

        exception.Message.Should().Be(ExceptionMessages.StartdatumIsAfterEinddatum);
    }

    [Fact]
    public async ValueTask With_TeCorrigeren_Einddatum_Then_It_Throws_StartdatumLigtNaEinddatum()
    {
        var teSchorsenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var startDatum = NullOrEmpty<DateOnly>.Create(_scenario.ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(1));
        var eindDatum = NullOrEmpty<DateOnly>.Create(_scenario.ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(-1));

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with
            {
                ErkenningId = teSchorsenErkenningId,
                StartDatum = startDatum,
                EindDatum = eindDatum,
            },
        };

        var exception = await Assert.ThrowsAsync<StartdatumLigtNaEinddatum>(async () =>
        {
            var commandMetadata = _fixture.Create<CommandMetadata>() with
            {
                Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode
            };

            await _commandHandler.Handle(
                new CommandEnvelope<
                    WijzigErkenningCommand>(
                    command,
                    commandMetadata)
            );
        });

        exception.Message.Should().Be(ExceptionMessages.StartdatumIsAfterEinddatum);
    }
}
