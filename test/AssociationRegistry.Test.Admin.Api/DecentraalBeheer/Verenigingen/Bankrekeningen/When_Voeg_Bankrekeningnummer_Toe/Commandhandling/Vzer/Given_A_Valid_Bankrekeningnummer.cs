namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Voeg_Bankrekeningnummer_Toe.Commandhandling.Vzer;

using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Events;
using Xunit;

public class Given_A_Valid_Bankrekeningnummer
{
    private readonly VoegBankrekeningnummerToeContext<FeitelijkeVerenigingWerdGeregistreerdScenario> _ctx = new(
        new FeitelijkeVerenigingWerdGeregistreerdScenario()
    );

    [Fact]
    public async ValueTask Then_A_BankrekeningWerdToegevoegd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdToegevoegd(
                1,
                command.Bankrekeningnummer.Iban.Value,
                command.Bankrekeningnummer.Doel,
                command.Bankrekeningnummer.Titularissen.Value
            )
        );
    }
}
