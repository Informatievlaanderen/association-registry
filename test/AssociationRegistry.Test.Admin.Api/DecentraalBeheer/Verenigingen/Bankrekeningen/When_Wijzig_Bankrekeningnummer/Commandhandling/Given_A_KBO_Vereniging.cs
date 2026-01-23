namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Wijzig_Bankrekeningnummer.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.WijzigBankrekening;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Xunit;

public class Given_A_KBO_Vereniging
{
    private readonly WijzigBankrekeningnummerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_A_KBO_Vereniging()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _commandHandler = new WijzigBankrekeningnummerCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask With_All_Fields_Then_It_Saves_A_BankrekeningnummerWerdGewijzigd_Event()
    {
        var teWijzigenBankrekeningnummerId = _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1.BankrekeningnummerId;

        var command = _fixture.Create<WijzigBankrekeningnummerCommand>() with
        {
            VCode = _scenario.VCode,
            Bankrekeningnummer = _fixture.Create<TeWijzigenBankrekeningnummer>() with
            {
                BankrekeningnummerId = teWijzigenBankrekeningnummerId,
            }
        };

        await _commandHandler.Handle(new CommandEnvelope<WijzigBankrekeningnummerCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdGewijzigd(
                teWijzigenBankrekeningnummerId,
                command.Bankrekeningnummer.Doel,
                command.Bankrekeningnummer.Titularis
            )
        );
    }

    [Fact]
    public async ValueTask With_Only_Doel_Then_It_Saves_A_BankrekeningnummerWerdGewijzigd_Event()
    {
        var teWijzigenBankrekeningnummer = _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1;

        var command = _fixture.Create<WijzigBankrekeningnummerCommand>() with
        {
            VCode = _scenario.VCode,
            Bankrekeningnummer = _fixture.Create<TeWijzigenBankrekeningnummer>() with
            {
                BankrekeningnummerId = teWijzigenBankrekeningnummer.BankrekeningnummerId,
                Titularis = null,
            }
        };

        await _commandHandler.Handle(new CommandEnvelope<WijzigBankrekeningnummerCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdGewijzigd(
                teWijzigenBankrekeningnummer.BankrekeningnummerId,
                command.Bankrekeningnummer.Doel,
                string.Empty //previous value
            )
        );
    }

    [Fact]
    public async ValueTask With_Only_Titularis_Then_It_Saves_A_BankrekeningnummerWerdGewijzigd_Event()
    {
        var teWijzigenBankrekeningnummerId = _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1.BankrekeningnummerId;

        var command = _fixture.Create<WijzigBankrekeningnummerCommand>() with
        {
            VCode = _scenario.VCode,
            Bankrekeningnummer = _fixture.Create<TeWijzigenBankrekeningnummer>() with
            {
                BankrekeningnummerId = teWijzigenBankrekeningnummerId,
                Doel = null,
            }
        };

        await _commandHandler.Handle(new CommandEnvelope<WijzigBankrekeningnummerCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdGewijzigd(
                teWijzigenBankrekeningnummerId,
                string.Empty, // previous value
                command.Bankrekeningnummer.Titularis
            )
        );
    }

    [Fact]
    public async ValueTask With_All_Null_Then_Nothing()
    {
        var teWijzigenBankrekeningnummerId = _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1.BankrekeningnummerId;

        var command = _fixture.Create<WijzigBankrekeningnummerCommand>() with
        {
            VCode = _scenario.VCode,
            Bankrekeningnummer = _fixture.Create<TeWijzigenBankrekeningnummer>() with
            {
                BankrekeningnummerId = teWijzigenBankrekeningnummerId,
                Doel = null,
                Titularis = null,
            }
        };

        await _commandHandler.Handle(new CommandEnvelope<WijzigBankrekeningnummerCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
