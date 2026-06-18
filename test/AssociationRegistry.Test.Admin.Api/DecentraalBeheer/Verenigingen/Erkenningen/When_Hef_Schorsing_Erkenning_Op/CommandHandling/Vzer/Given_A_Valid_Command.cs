namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_Valid_Command
{
    private readonly HefSchorsingErkenningOpContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_Saves_An_SchorsingVanErkenningWerdOpgeheven_Event()
    {
        var command = _ctx.CreateCommand();

        var status = ErkenningStatus.Bepaal(
            ErkenningsPeriode.Create(
                _ctx.Scenario.ErkenningWerdGeregistreerd.Startdatum,
                _ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum
            ),
            DateOnly.FromDateTime(DateTime.Now)
        );

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new SchorsingVanErkenningWerdOpgeheven(command.ErkenningId, status.Value)
        );
    }
}
