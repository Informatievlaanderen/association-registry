namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_Verlopen_Geschorste_Erkenning
{
    private readonly HefSchorsingErkenningOpContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVerlopenGeschorsteErkenningScenario> _ctx =
        new(
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVerlopenGeschorsteErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode
        );

    [Fact]
    public async ValueTask With_Previous_State_Verlopen_Then_Saves_An_SchorsingVanErkenningWerdOpgeheven_Event()
    {
        var command = _ctx.HefSchorsingErkenningOpCommand;

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new SchorsingVanErkenningWerdOpgeheven(command.ErkenningId, ErkenningStatus.Verlopen.Value)
        );
    }
}
