namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Wijzig_Bankrekeningnummer.Commandhandling.Vzer;

using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_An_Unknown_Bankrekeningnummer
{
    private readonly WijzigBankrekeningnummerContext<BankrekeningnummerWerdToegevoegdScenario> _ctx = new(
        new BankrekeningnummerWerdToegevoegdScenario(),
        s => s.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId
    );

    [Fact]
    public async ValueTask Then_Throws_BankrekeningnummerNietGekend()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdGewijzigd(
                _ctx.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                command.Bankrekeningnummer.Doel,
                command.Bankrekeningnummer.Titularissen
            )
        );
    }
}
