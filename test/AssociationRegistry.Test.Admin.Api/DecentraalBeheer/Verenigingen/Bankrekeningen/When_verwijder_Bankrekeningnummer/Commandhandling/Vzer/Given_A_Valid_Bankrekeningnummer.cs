namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_verwijder_Bankrekeningnummer.Commandhandling.Vzer;

using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_Valid_Bankrekeningnummer
{
    private readonly VerwijderBankrekeningnummerContext<BankrekeningnummerWerdToegevoegdScenario> _ctx =
        new(
            new BankrekeningnummerWerdToegevoegdScenario(),
            s => s.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId
        );

    [Fact]
    public async ValueTask Then_A_BankrekeningWerdToegevoegd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdVerwijderd(
                command.BankrekeningnummerId,
                _ctx.Scenario.BankrekeningnummerWerdToegevoegd.Iban
            )
        );
    }
}
