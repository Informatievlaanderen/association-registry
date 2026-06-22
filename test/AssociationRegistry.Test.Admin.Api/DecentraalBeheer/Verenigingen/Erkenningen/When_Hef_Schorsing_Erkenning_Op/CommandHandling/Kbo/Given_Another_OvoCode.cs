namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Another_OvoCode
{
    private readonly HefSchorsingErkenningOpContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId);

    [Fact]
    public async ValueTask Then_Throws_GiIsNietBevoegd()
    {
        var command = _ctx.HefSchorsingErkenningOpCommand;
        var metadata = _ctx.CreateMetadata();

        var exception = await Assert.ThrowsAsync<GiIsNietBevoegd>(async () => await _ctx.Handle(command, metadata));

        exception.Message.Should().Be(ExceptionMessages.GiIsNietBevoegd);
    }
}
