namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Schors_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Another_OvoCode
{
    private readonly SchorsErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId);

    [Fact]
    public async ValueTask Then_Throws_GiIsNIetBevoegd()
    {
        var command = _ctx.SchorsErkenningCommand;
        var metadata = _ctx.CreateMetadata();

        var exception = await Assert.ThrowsAsync<GiIsNietBevoegd>(async () => await _ctx.Handle(command, metadata));

        exception.Message.Should().Be(string.Format(ExceptionMessages.GiIsNietBevoegd, _ctx.Scenario.ErkenningWerdGeregistreerd.ErkenningId));
    }
}
