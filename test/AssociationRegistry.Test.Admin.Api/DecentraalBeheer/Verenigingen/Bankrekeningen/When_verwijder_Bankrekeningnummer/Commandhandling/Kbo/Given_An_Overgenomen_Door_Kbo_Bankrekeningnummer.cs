namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_verwijder_Bankrekeningnummer.Commandhandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_An_Overgenomen_Door_Kbo_Bankrekeningnummer
{
    private readonly VerwijderBankrekeningnummerContext<BankrekeningnummerWerdOvergenomenVanuitKBOScenario> _ctx =
        new(
            new BankrekeningnummerWerdOvergenomenVanuitKBOScenario(),
            s => s.BankrekeningnummerWerdOvergenomenVanuitKBO.BankrekeningnummerId
        );

    [Fact]
    public async ValueTask Then_Throws()
    {
        var command = _ctx.CreateCommand();

        var exception = await Assert.ThrowsAsync<ActieIsNietToegestaanVoorKboBankrekeningnummer>(async () =>
            await _ctx.Handle(command)
        );

        exception.Message.Should().Be(ExceptionMessages.UnsupportedOperationForKboBankrekeningnummer);
    }
}
