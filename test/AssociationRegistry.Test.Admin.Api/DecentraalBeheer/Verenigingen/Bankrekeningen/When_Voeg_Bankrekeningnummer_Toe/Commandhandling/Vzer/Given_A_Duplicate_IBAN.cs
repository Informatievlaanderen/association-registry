namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Voeg_Bankrekeningnummer_Toe.Commandhandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_A_Duplicate_IBAN
{
    private readonly VoegBankrekeningnummerToeContext<BankrekeningnummerWerdToegevoegdScenario> _ctx =
        new(new BankrekeningnummerWerdToegevoegdScenario());

    [Fact]
    public async ValueTask Then_An_IbanMoetUniekZijn_Exception_Is_Thrown()
    {
        var command = _ctx.CreateCommand(
            iban: IbanNummer.Create(_ctx.Scenario.BankrekeningnummerWerdToegevoegd.Iban)
        );

        var exception = await Assert.ThrowsAsync<IbanMoetUniekZijn>(async () =>
            await _ctx.Handle(command)
        );

        exception.Message.Should().Be(ExceptionMessages.IbanMoetUniekZijn);
    }
}
