namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Maak_Validatie_Bankrekeningnummer_Ongedaan.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_OvoCode_Not_In_BevestigdDoor
{
    private readonly MaakValidatieBankrekeningnummerOngedaanContext<BankrekeningnummerWerdToegevoegdScenario> _ctx =
        new(
            new BankrekeningnummerWerdToegevoegdScenario(),
            s => s.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId
        );

    [Fact]
    public async ValueTask Then_Throws_ValidatieBankrekeningnummerIsNietGekend()
    {
        var command = _ctx.CreateCommand();

        var exception = await Assert.ThrowsAsync<ValidatieBankrekeningnummerIsNietGekend>(async () =>
            await _ctx.Handle(command)
        );

        exception
            .Message.Should()
            .Be(string.Format(ExceptionMessages.ValidatieBankrekeningnummerIsNietGekend, _ctx.Metadata.Initiator));
    }
}
