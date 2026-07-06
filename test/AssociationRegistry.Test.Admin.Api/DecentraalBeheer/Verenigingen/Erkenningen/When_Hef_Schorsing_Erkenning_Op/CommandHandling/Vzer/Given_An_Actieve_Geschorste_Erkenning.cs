namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_An_Actieve_Geschorste_Erkenning
{
    private readonly HefSchorsingErkenningOpContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithActieveGeschorsteErkenningScenario> _ctx =
        new(
            new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithActieveGeschorsteErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode
        );

    [Fact]
    public async ValueTask With_Previous_State_Actief_Then_Saves_An_SchorsingVanErkenningWerdOpgeheven_Event()
    {
        var command = _ctx.HefSchorsingErkenningOpCommand;

        var status = ErkenningStatus.Bepaal(
            ErkenningsPeriode.Create(
                _ctx.Scenario.ErkenningWerdGeregistreerd.Startdatum,
                _ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum
            ),
            DateOnly.FromDateTime(DateTime.Now)
        );

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new SchorsingVanErkenningWerdOpgeheven(command.ErkenningId, ErkenningStatus.Actief.Value),
            new VerenigingWerdErkend()
        );
    }
}
