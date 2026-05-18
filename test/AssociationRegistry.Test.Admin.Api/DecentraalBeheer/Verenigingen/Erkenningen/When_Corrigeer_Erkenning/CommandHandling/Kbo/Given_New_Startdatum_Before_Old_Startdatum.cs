namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Primitives;
using Xunit;

public class Given_New_Startdatum_Before_Old_Startdatum
{
    private readonly CorrigeerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningInAanvraagScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_New_Startdatum_Before_Old_Startdatum()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningInAanvraagScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new CorrigeerErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_It_Adds_An_ErkenningWerdGecorrigeerd_Event_With_Status_Actief()
    {
        var teSchorsenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;
        var today = DateOnly.FromDateTime(DateTime.Today);
        var pastWeek = today.AddDays(-7);
        var nextWeek = today.AddDays(7);

        var command = _fixture.Create<CorrigeerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeCorrigerenErkenning>() with
            {
                ErkenningId = teSchorsenErkenningId,
                StartDatum = NullOrEmpty<DateOnly>.Create(pastWeek),
                Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(today),
                EindDatum = NullOrEmpty<DateOnly>.Create(nextWeek),
            },
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        await _commandHandler.Handle(new CommandEnvelope<CorrigeerErkenningCommand>(command, commandMetadata));

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new ErkenningWerdGecorrigeerd(
                command.Erkenning.ErkenningId,
                command.Erkenning.StartDatum.Value,
                command.Erkenning.EindDatum.Value,
                command.Erkenning.Hernieuwingsdatum.Value,
                command.Erkenning.HernieuwingsUrl,
                ErkenningStatus.Actief.Value
            )
        );
    }
}
