namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_Geschorste_Erkenning
{
    private readonly WijzigErkenningContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_It_Adds_An_ErkenningWerdGewijzigd_Event_With_Status_Geschorst()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGewijzigd(
                command.Erkenning.ErkenningId,
                command.Erkenning.StartDatum.Value,
                command.Erkenning.EindDatum.Value,
                command.Erkenning.Hernieuwingsdatum.Value,
                command.Erkenning.HernieuwingsUrl,
                ErkenningStatus.Geschorst.Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }
}
