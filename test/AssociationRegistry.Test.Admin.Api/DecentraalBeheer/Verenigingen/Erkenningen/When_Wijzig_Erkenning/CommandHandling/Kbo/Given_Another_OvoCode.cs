namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Another_OvoCode
{
    private readonly WijzigErkenningContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId);

    [Fact]
    public async ValueTask Then_It_Saves_An_ErkenningWerdGeschorst_Event()
    {
        var command = _ctx.CreateCommand();
        var metadata = _ctx.CreateMetadata();

        var exception = await Assert.ThrowsAsync<GiIsNietBevoegd>(async () => await _ctx.Handle(command, metadata));

        exception.Message.Should().Be(ExceptionMessages.GiIsNietBevoegd);
    }
}
