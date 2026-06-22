namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.
    When_Corrigeer_Reden_Schorsing_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Invalid_Reden_Erkenning
{
    private readonly CorrigeerRedenSchorsingErkenningContext<
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async ValueTask Then_Throw_ErkenningRedenSchorsingVerplicht(string reden)
    {
        var command = _ctx.CorrigeerRedenSchorsingErkenningCommand with
        {
            Erkenning = _ctx.CorrigeerRedenSchorsingErkenningCommand.Erkenning with
            {
                RedenSchorsing = reden,
            },
        };

        var exception = await Assert.ThrowsAsync<ErkenningRedenSchorsingIsVerplicht>(async () =>
        {
            await _ctx.Handle(command);
        });

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningRedenSchorsingVerplicht));
    }
}
