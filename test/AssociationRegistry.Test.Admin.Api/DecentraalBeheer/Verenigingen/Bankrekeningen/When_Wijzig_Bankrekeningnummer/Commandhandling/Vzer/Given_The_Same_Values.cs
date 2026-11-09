namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Wijzig_Bankrekeningnummer.Commandhandling.Vzer;

using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Xunit;

public class Given_The_Same_Values
{
    private readonly WijzigBankrekeningnummerContext<BankrekeningnummerWerdToegevoegdScenario> _ctx = new(
        new BankrekeningnummerWerdToegevoegdScenario(),
        s => s.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId
    );

    [Fact]
    public async ValueTask With_Same_Doel_Then_Nothing()
    {
        var command = _ctx.CreateCommand(b =>
            b with
            {
                Doel = _ctx.Scenario.BankrekeningnummerWerdToegevoegd.Doel,
                Titularissen = null,
            }
        );

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
    }

    [Fact]
    public async ValueTask With_Same_Titularissen_Then_Nothing()
    {
        var command = _ctx.CreateCommand(b =>
            b with
            {
                Doel = null,
                Titularissen = _ctx.Scenario.BankrekeningnummerWerdToegevoegd.Titularissen.ToArray(),
            }
        );

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
    }

    [Fact]
    public async ValueTask With_Same_Doel_And_Titularissen_Then_Nothing()
    {
        var command = _ctx.CreateCommand(b =>
            b with
            {
                Doel = _ctx.Scenario.BankrekeningnummerWerdToegevoegd.Doel,
                Titularissen = _ctx.Scenario.BankrekeningnummerWerdToegevoegd.Titularissen.ToArray(),
            }
        );

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
    }
}
