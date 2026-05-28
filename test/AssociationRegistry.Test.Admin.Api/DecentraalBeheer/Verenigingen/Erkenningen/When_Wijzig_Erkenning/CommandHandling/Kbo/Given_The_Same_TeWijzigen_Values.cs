namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Primitives;
using Xunit;

public class Given_The_Same_TeWijzigen_Values
{
    private readonly WijzigErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_The_Same_TeWijzigen_Values()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new WijzigErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Should_Not_Have_Any_Saves()
    {
        var teWijzigenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with
            {
                ErkenningId = teWijzigenErkenningId,
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
                WijzigErkenningCommand>(
                command,
                commandMetadata)
        );

        _verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
