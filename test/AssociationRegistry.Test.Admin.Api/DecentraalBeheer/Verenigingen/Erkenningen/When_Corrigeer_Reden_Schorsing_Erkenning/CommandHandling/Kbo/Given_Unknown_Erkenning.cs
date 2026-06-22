namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.
    When_Corrigeer_Reden_Schorsing_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Unknown_Erkenning
{
    private readonly CorrigeerRedenSchorsingErkenningContext<
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_Throws_ErkenningIsNietGekend()
    {
        var unknownErkenningId = _ctx.CreateUnknownErkenningId();

        var command = _ctx.CorrigeerRedenSchorsingErkenningCommand with
        {
            Erkenning = _ctx.CorrigeerRedenSchorsingErkenningCommand.Erkenning with
            {
                ErkenningId = unknownErkenningId,
            },
        };

        var exception = await Assert.ThrowsAsync<ErkenningIsNietGekend>(async () =>
        {
            await _ctx.Handle(command);
        });

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningIsNietGekend, unknownErkenningId));
    }
}
