namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Wijzig_Bankrekeningnummer.Commandhandling.Vzer;

using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_Valid_Command
{
    private readonly WijzigBankrekeningnummerContext<BankrekeningnummerWerdToegevoegdScenario> _ctx =
        new(
            new BankrekeningnummerWerdToegevoegdScenario(),
            s => s.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId
        );

    [Fact]
    public async ValueTask With_All_Fields_Then_It_Saves_A_BankrekeningnummerWerdGewijzigd_Event()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdGewijzigd(
                _ctx.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                command.Bankrekeningnummer.Doel,
                command.Bankrekeningnummer.Titularis
            )
        );
    }

    [Fact]
    public async ValueTask With_Only_Doel_Then_It_Saves_A_BankrekeningnummerWerdGewijzigd_Event()
    {
        var command = _ctx.CreateCommand(b => b with { Titularis = null });

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdGewijzigd(
                _ctx.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                command.Bankrekeningnummer.Doel,
                _ctx.Scenario.BankrekeningnummerWerdToegevoegd.Titularis
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
                _ctx.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                _ctx.Scenario.BankrekeningnummerWerdToegevoegd.Doel,
                command.Bankrekeningnummer.Titularis
            )
        );
    }

    [Fact]
    public async ValueTask With_All_Null_Then_Nothing()
    {
        var command = _ctx.CreateCommand(b => b with { Doel = null, Titularis = null });

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
    }
}
