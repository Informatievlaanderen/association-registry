namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Valideer_Bankrekeningnummer.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Unknown_Bankrekeningnummer
{
    private readonly ValideerBankrekeningnummerContext<BankrekeningnummerWerdToegevoegdScenario> _ctx = new(
        new BankrekeningnummerWerdToegevoegdScenario(),
        s => s.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId
    );

    [Fact]
    public async ValueTask Then_Throws_BankrekeningnummerIsNietGekend()
    {
        var command = _ctx.CreateCommand(bankrekeningnummerId: _ctx.CreateUnknownBankrekeningnummerId());

        var exception = await Assert.ThrowsAsync<BankrekeningnummerIsNietGekend>(async () =>
            await _ctx.Handle(command)
        );

        exception
            .Message.Should()
            .Be(string.Format(ExceptionMessages.BankrekeningnummerIsNietGekend, command.BankrekeningnummerId));
    }
}
