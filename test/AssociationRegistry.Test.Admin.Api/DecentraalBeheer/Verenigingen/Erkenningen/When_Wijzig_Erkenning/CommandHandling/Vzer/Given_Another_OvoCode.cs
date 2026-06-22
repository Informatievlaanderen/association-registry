namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Another_OvoCode
{
    private readonly WijzigErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId);

    [Fact]
    public async ValueTask Then_It_Saves_An_ErkenningWerdGeschorst_Event()
    {
        var command = _ctx.WijzigErkenningCommand;
        var metadata = _ctx.CreateMetadata();

        var exception = await Assert.ThrowsAsync<GiIsNietBevoegd>(async () => await _ctx.Handle(command, metadata));

        exception.Message.Should().Be(ExceptionMessages.GiIsNietBevoegd);
    }
}
