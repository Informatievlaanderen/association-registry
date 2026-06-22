namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Schors_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Invalid_Reden_Erkenning
{
    private readonly SchorsErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async ValueTask Then_Throw_ErkenningRedenSchorsingVerplicht(string reden)
    {
        var command = _ctx.SchorsErkenningCommand with
        {
            Erkenning = _ctx.SchorsErkenningCommand.Erkenning with
            {
                RedenSchorsing = reden,
            }
        };

        var exception = await Assert.ThrowsAsync<ErkenningRedenSchorsingIsVerplicht>(async () => await _ctx.Handle(command));

        exception.Message.Should().Be(ExceptionMessages.ErkenningRedenSchorsingVerplicht);
    }
}
