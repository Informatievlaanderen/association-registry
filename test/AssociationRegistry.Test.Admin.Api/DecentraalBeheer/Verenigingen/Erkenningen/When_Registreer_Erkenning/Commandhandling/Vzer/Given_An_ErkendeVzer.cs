namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_An_ErkendeVzer
{
    private readonly RegistreerErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdErkendScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdErkendScenario());

    [Fact]
    public async ValueTask With_Actieve_Erkenning_Then_ErkenningWerdGeregistreerd_With_The_Next_Id_And_No_Verening_Werd_Erkend()
    {
        var command = _ctx.RegistreerErkenningCommand with
        {
            Erkenning = _ctx.RegistreerErkenningCommand.Erkenning with
            {
                ErkenningsPeriode = ErkenningsPeriode.Create(startdatum: null, einddatum: null),
            },
        };

        var ipdcProduct = _ctx.CreateIpdcProduct();
        var initiator = _ctx.CreateInitiator();

        await _ctx.Handle(command, ipdcProduct, initiator);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGeregistreerd(
                ErkenningId: 1,
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

    [Fact]
    public async ValueTask With_Niet_Actieve_Erkenning_Then_ErkenningWerdGeregistreerd_With_The_Next_Id_And_No_Verening_Werd_Erkend()
    {
        var today = DateTime.Now;
        var startdatumInFuture = DateOnly.FromDateTime(today.AddDays(_ctx.Fixture.Create<int>()));

        var command = _ctx.RegistreerErkenningCommand with
        {
            Erkenning = _ctx.RegistreerErkenningCommand.Erkenning with
            {
                ErkenningsPeriode = ErkenningsPeriode.Create(startdatumInFuture, einddatum: null),
            },
        };

        var ipdcProduct = _ctx.CreateIpdcProduct();
        var initiator = _ctx.CreateInitiator();

        await _ctx.Handle(command, ipdcProduct, initiator);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGeregistreerd(
                ErkenningId: 1,
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
