namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_An_InAanvraag_Geschorste_Erkenning
{
    private readonly HefSchorsingErkenningOpContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithInAanvraagGeschorsteErkenningScenario> _ctx =
        new(
            new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithInAanvraagGeschorsteErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode
        );

    [Fact]
    public async ValueTask With_Previous_State_InAanvraag_Then_Saves_An_SchorsingVanErkenningWerdOpgeheven_Event()
    {
        var command = _ctx.HefSchorsingErkenningOpCommand;

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new SchorsingVanErkenningWerdOpgeheven(command.ErkenningId, ErkenningStatus.InAanvraag.Value)
        );
    }
}
