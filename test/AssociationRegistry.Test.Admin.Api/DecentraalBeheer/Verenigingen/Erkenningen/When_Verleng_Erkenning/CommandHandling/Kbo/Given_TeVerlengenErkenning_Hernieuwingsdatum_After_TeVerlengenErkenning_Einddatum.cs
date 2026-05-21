namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Verleng_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerlengErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Primitives;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Xunit;

public class Given_TeVerlengenErkenning_Hernieuwingsdatum_After_TeVerlengenErkenning_Einddatum
{
    private readonly VerlengErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_TeVerlengenErkenning_Hernieuwingsdatum_After_TeVerlengenErkenning_Einddatum()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new VerlengErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Throw_HernieuwingsDatumMoetTussenStartEnEindDatumLiggen()
    {
        var erkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var commandEnvelope = CreateCommandWithHernieuwingsdatumLaterThanEinddatum(erkenningId);

        var exception = await Assert.ThrowsAsync<HernieuwingsDatumMoetTussenStartEnEindDatumLiggen>(async () =>
        {
            await _commandHandler.Handle(commandEnvelope);
        });

        exception.Message.Should()
                 .Be(string.Format(ExceptionMessages.HernieuwingsDatumMoetTussenStartEnEindDatumLiggen, erkenningId));
    }

    private CommandEnvelope<VerlengErkenningCommand> CreateCommandWithHernieuwingsdatumLaterThanEinddatum(int erkenningId)
    {
        var newEinddatum =  _scenario.ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(_fixture.Create<int>());

        var hernieuwingsdatumAfterEinddatum = newEinddatum.AddDays(_fixture.Create<int>());
            var command = _fixture.Create<VerlengErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeVerlengenErkenning>() with
            {
                ErkenningId = erkenningId,
                Einddatum = newEinddatum,
                Hernieuwingsdatum =NullOrEmpty<DateOnly>.Create(hernieuwingsdatumAfterEinddatum),
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
