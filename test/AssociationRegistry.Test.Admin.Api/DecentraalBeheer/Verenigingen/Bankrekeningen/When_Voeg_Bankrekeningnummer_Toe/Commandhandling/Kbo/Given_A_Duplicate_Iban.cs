namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Voeg_Bankrekeningnummer_Toe.Commandhandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_A_Duplicate_Iban
{
    private readonly VoegBankrekeningnummerToeContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario());

    [Fact]
    public async ValueTask Then_Throws()
    {
        var command = _ctx.CreateCommand(
            iban: IbanNummer.Hydrate(_ctx.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1.Iban)
        );

        var exception = await Assert.ThrowsAsync<IbanMoetUniekZijn>(async () =>
            await _ctx.Handle(command)
        );

        exception.Message.Should().Be(ExceptionMessages.IbanMoetUniekZijn);
    }
}
