namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Wijzig_Bankrekeningnummer.Commandhandling.Kbo;

using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_Valid_Command
{
    private readonly WijzigBankrekeningnummerContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario> _ctx =
        new(
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario(),
            s => s.BankrekeningnummerWerdToegevoegdVanuitKBO1.BankrekeningnummerId
        );

    [Fact]
    public async ValueTask With_All_Fields_Then_It_Saves_A_BankrekeningnummerWerdGewijzigd_Event()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdGewijzigd(
                _ctx.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1.BankrekeningnummerId,
                command.Bankrekeningnummer.Doel,
                command.Bankrekeningnummer.Titularissen
            )
        );
    }

    [Fact]
    public async ValueTask With_Only_Doel_Then_It_Saves_A_BankrekeningnummerWerdGewijzigd_Event()
    {
        var command = _ctx.CreateCommand(b => b with { Titularissen = null });

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdGewijzigd(
                _ctx.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1.BankrekeningnummerId,
                command.Bankrekeningnummer.Doel,
                [] //previous value
            )
        );
    }

    [Fact]
    public async ValueTask With_Only_Titularis_Then_It_Saves_A_BankrekeningnummerWerdGewijzigd_Event()
    {
        var command = _ctx.CreateCommand(b => b with { Doel = null });

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdGewijzigd(
                _ctx.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1.BankrekeningnummerId,
                string.Empty, // previous value
                command.Bankrekeningnummer.Titularissen
            )
        );
    }

    [Fact]
    public async ValueTask With_All_Null_Then_Nothing()
    {
        var command = _ctx.CreateCommand(b => b with { Doel = null, Titularissen = null });

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
    }
}
