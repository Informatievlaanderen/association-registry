namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Reden_Schorsing_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Invalid_Reden_Erkenning
{
    private readonly CorrigeerRedenSchorsingErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async ValueTask Then_Throw_ErkenningRedenSchorsingVerplicht(string reden)
    {
        var command = _ctx.CreateCommand(redenSchorsing: reden);

        var exception = await Assert.ThrowsAsync<ErkenningRedenSchorsingIsVerplicht>(async () =>
        {
            await _ctx.Handle(command);
        });

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningRedenSchorsingVerplicht));
    }
}
