namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_Valid_Erkenning
{
    private readonly RegistreerErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario());

    [Fact]
    public async ValueTask Then_An_ErkenningWerdGeregistreerd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = _ctx.CreateCommand();
        var ipdcProduct = _ctx.CreateIpdcProduct();
        var initiator = _ctx.CreateInitiator();

        await _ctx.Handle(command, ipdcProduct, initiator);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGeregistreerd(
                1,
                ipdcProduct,
                command.Erkenning.ErkenningsPeriode.Startdatum,
                command.Erkenning.ErkenningsPeriode.Einddatum,
                command.Erkenning.Hernieuwingsdatum.Value,
                command.Erkenning.HernieuwingsUrl.Value,
                initiator,
                ErkenningStatus
                    .Bepaal(
                        ErkenningsPeriode.Create(
                            command.Erkenning.ErkenningsPeriode.Startdatum,
                            command.Erkenning.ErkenningsPeriode.Einddatum
                        ),
                        DateOnly.FromDateTime(DateTime.Now)
                    )
                    .Value
            )
        );
    }
}
