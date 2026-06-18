namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Reden_Schorsing_Erkenning.CommandHandling.Kbo;

using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Xunit;

public class Given_Erkenning_Geschorst_Met_Dezelfde_Reden
{
    private readonly CorrigeerRedenSchorsingErkenningContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_Nothing()
    {
        var command = _ctx.CreateCommand(redenSchorsing: _ctx.Scenario.ErkenningWerdGeschorst.RedenSchorsing);
        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
    }
}
