namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_verwijder_Bankrekeningnummer.Commandhandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Already_Removed_Bankrekeningnummer
{
    private readonly VerwijderBankrekeningnummerContext<BankrekeningnummerWerdVerwijderdScenario> _ctx =
        new(
            new BankrekeningnummerWerdVerwijderdScenario(),
            s => s.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId
        );

    [Fact]
    public async ValueTask Then_A_BankrekeningnummerIsNietGekend_Exception_Is_Thrown()
    {
        var command = _ctx.CreateCommand();

        var exception = await Assert.ThrowsAsync<BankrekeningnummerIsNietGekend>(async () =>
            await _ctx.Handle(command)
        );

        _ctx.AggregateSessionMock.ShouldNotHaveAnySaves();

        exception
            .Message.Should()
            .Be(
                string.Format(
                    ExceptionMessages.BankrekeningnummerIsNietGekend,
                    _ctx.Scenario.BankrekeningnummerWerdVerwijderd.BankrekeningnummerId
                )
            );
    }
}
