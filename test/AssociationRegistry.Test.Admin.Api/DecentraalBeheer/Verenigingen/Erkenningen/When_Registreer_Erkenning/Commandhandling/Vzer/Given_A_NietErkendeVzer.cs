namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_NietErkendeVzer
{
    private readonly RegistreerErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario());

    [Fact]
    public async ValueTask With_Actieve_Erkenning_Then_ErkenningWerdGeregistreerd_With_The_Next_Id_And_Verening_Werd_Erkend()
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
            _ctx.ErkenningWerdGeregistreerd(command, Erkenningen.InitialId, ipdcProduct, initiator),
            new VerenigingWerdErkend()
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
            _ctx.ErkenningWerdGeregistreerd(command, Erkenningen.InitialId, ipdcProduct, initiator)
        );
    }
}
