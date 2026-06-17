namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_verwijder_Bankrekeningnummer.Commandhandling.Kbo;

using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_Gi_Bankrekeningnummer
{
    private readonly VerwijderBankrekeningnummerContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersAddedByGIScenario> _ctx =
        new(
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersAddedByGIScenario(),
            s => s.GIBankrekeningnummerWerdToegevoegd.BankrekeningnummerId
        );

    [Fact]
    public async ValueTask Then_A_BankrekeningWerdToegevoegd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdVerwijderd(
                command.BankrekeningnummerId,
                _ctx.Scenario.GIBankrekeningnummerWerdToegevoegd.Iban
            )
        );
    }
}
