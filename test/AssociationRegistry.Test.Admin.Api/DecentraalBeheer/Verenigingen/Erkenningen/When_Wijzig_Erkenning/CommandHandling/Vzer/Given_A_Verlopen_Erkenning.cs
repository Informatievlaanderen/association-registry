namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Vzer;

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

public class Given_A_Verlopen_Erkenning
{
    private readonly WijzigErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_A_Verlopen_Erkenning()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new WijzigErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask With_Startdatum_In_Past_And_Einddatum_In_Future_Then_Status_Actief()
    {
        var teWijzigenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var today = DateOnly.FromDateTime(DateTime.Now);
        var startdatum = today.AddDays(-_fixture.Create<int>());
        var herniewingsdatum = today.AddDays(_fixture.Create<int>());
        var einddatum = herniewingsdatum.AddDays(_fixture.Create<int>());

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with
            {
                ErkenningId = teWijzigenErkenningId,
                StartDatum = NullOrEmpty<DateOnly>.Create(startdatum),
                EindDatum = NullOrEmpty<DateOnly>.Create(einddatum),
                Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(herniewingsdatum),
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
                command.Erkenning.EindDatum.Value,
                command.Erkenning.Hernieuwingsdatum.Value,
                _scenario.ErkenningWerdGeregistreerd.HernieuwingsUrl,
                ErkenningStatus.Actief.Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }
}
