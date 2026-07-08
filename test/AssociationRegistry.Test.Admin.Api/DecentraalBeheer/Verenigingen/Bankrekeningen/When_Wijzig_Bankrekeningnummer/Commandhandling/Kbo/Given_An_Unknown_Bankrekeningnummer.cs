namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Wijzig_Bankrekeningnummer.Commandhandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using AssociationRegistry.Resources;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Xunit;

public class Given_An_Unknown_Bankrekeningnummer
{
    private readonly WijzigBankrekeningnummerContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario> _ctx =
        new(
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario(),
            s => s.BankrekeningnummerWerdToegevoegdVanuitKBO1.BankrekeningnummerId
        );

    [Fact]
    public async ValueTask Then_Throws_BankrekeningnummerNietGekend()
    {
        var command = _ctx.CreateCommand(bankrekeningnummerId: _ctx.GetUnknownBankrekeningnummerId);

        var exception = await Assert.ThrowsAsync<BankrekeningnummerIsNietGekend>(async () =>
            await _ctx.Handle(command)
        );
        exception
            .Message.Should()
            .Be(string.Format(ExceptionMessages.BankrekeningnummerIsNietGekend, _ctx.GetUnknownBankrekeningnummerId));
    }
}
