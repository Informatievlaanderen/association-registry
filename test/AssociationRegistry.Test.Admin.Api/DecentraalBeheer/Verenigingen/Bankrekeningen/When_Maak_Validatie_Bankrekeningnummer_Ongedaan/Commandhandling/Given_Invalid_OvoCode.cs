namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Maak_Validatie_Bankrekeningnummer_Ongedaan.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.ValideerBankrekening;
using FluentAssertions;
using Xunit;

public class Given_Invalid_OvoCode
{
    private readonly MaakValidatieBankrekeningnummerOngedaanContext<BankrekeningnummerWerdToegevoegdScenario> _ctx =
        new(
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
