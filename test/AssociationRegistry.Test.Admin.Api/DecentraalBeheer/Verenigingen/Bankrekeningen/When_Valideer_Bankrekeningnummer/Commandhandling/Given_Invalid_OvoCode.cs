namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Valideer_Bankrekeningnummer.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Invalid_OvoCode
{
    private readonly ValideerBankrekeningnummerContext<BankrekeningnummerWerdToegevoegdScenario> _ctx = new(
        new BankrekeningnummerWerdToegevoegdScenario(),
        s => s.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId
    );

    [Fact]
    public async ValueTask Then_Throws_BankrekeningnummerIsNietGekend()
    {
        var command = _ctx.CreateCommand(bankrekeningnummerId: _ctx.CreateUnknownBankrekeningnummerId());
        var metadata = _ctx.CreateMetadata(initiator: WellknownOvoNumbers.VloOvoCode);

        var exception = await Assert.ThrowsAsync<OvoCodeIsNietToegelatenDezeActieUitTeVoeren>(async () =>
            await _ctx.Handle(command, metadata)
        );

        exception
            .Message.Should()
            .Be(
                string.Format(
                    ExceptionMessages.OvoCodeIsNietGemachtigdOmDezeActieUitTeVoeren,
                    WellknownOvoNumbers.VloOvoCode
                )
            );
    }
}
