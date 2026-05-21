namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Verleng_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerlengErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_TeVerlengenEinddatum_Before_Current_Einddatum
{
    private readonly VerlengErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_TeVerlengenEinddatum_Before_Current_Einddatum()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new VerlengErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Throw_NieuweEinddatumMoetLaterZijnDanHuidigeEinddatum()
    {
        var erkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var commandEnvelope = CreateCommandWithTeVerlengenEinddatumBeforeCurrentEinddatum(erkenningId);

        var exception = await Assert.ThrowsAsync<NieuweEinddatumMoetLaterZijnDanHuidigeEinddatum>(async () =>
        {
            await _commandHandler.Handle(commandEnvelope);
        });

        exception.Message.Should()
                 .Be(string.Format(ExceptionMessages.NieuweEinddatumMoetLaterZijnDanHuidigeEinddatum, erkenningId));
    }

    private CommandEnvelope<VerlengErkenningCommand> CreateCommandWithTeVerlengenEinddatumBeforeCurrentEinddatum(int erkenningId)
    {
        var newEinddatumLowerThanGeregistreerdeEinddatum = _scenario.ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(-_fixture.Create<int>());

        var command = _fixture.Create<VerlengErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeVerlengenErkenning>() with
            {
                ErkenningId = erkenningId,
                Einddatum = newEinddatumLowerThanGeregistreerdeEinddatum,
            },
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        var commandEnvelope = new CommandEnvelope<VerlengErkenningCommand>(command, commandMetadata);

        return commandEnvelope;
    }
}
