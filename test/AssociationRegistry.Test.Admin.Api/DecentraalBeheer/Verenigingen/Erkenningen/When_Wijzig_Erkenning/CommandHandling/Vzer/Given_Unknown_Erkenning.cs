namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Unknown_Erkenning
{
    private readonly WijzigErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_Throws_ErkenningIsNietGekend()
    {
        var unknownErkenningId = _ctx.CreateUnknownErkenningId();
        var command = _ctx.CreateCommand(erkenningId: unknownErkenningId);

        var exception = await Assert.ThrowsAsync<ErkenningIsNietGekend>(async () => await _ctx.Handle(command));

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningIsNietGekend, unknownErkenningId));
    }
}
