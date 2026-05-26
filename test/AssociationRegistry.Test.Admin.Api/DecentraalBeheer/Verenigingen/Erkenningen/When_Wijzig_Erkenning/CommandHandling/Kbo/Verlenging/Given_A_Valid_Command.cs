namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.
    CommandHandling.Kbo.Verlenging;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Primitives;
using Xunit;

public class Given_A_Valid_Command
{
    private readonly WijzigErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_A_Valid_Command()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new WijzigErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask With_Nieuwe_Einddatum()
    {
        var teSchorsenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var nieuweEinddatum =
            NullOrEmpty<DateOnly>.Create(
                _scenario.ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(_fixture.Create<int>()));

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with
            {
                ErkenningId = teSchorsenErkenningId,
                EindDatum = nieuweEinddatum,
                WijzigingsType = WijzigingsTypeErkenning.VerlengValue,
                StartDatum = NullOrEmpty<DateOnly>.Null,
                Hernieuwingsdatum =  NullOrEmpty<DateOnly>.Null,
                HernieuwingsUrl = null
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
                _scenario.ErkenningWerdGeregistreerd.Status,
                command.Erkenning.WijzigingsType
            )
        );
    }
}
