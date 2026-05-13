namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Erkenning.
    CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Primitives;
using Xunit;

public class Given_The_Same_TeCorrigeren_Values
{
    private readonly CorrigeerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_The_Same_TeCorrigeren_Values()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new CorrigeerErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Should_Not_Have_Any_Saves()
    {
        var teCorrigerenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var command = _fixture.Create<CorrigeerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeCorrigerenErkenning>() with
            {
                ErkenningId = teCorrigerenErkenningId,
                StartDatum = NullOrEmpty<DateOnly>.Create(_scenario.ErkenningWerdGeregistreerd.Startdatum.Value),
                EindDatum = NullOrEmpty<DateOnly>.Create(_scenario.ErkenningWerdGeregistreerd.Einddatum.Value),
                Hernieuwingsdatum =
                NullOrEmpty<DateOnly>.Create(_scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value),
                HernieuwingsUrl = _scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
            },
        };

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

        _verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
