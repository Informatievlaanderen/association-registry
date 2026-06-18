namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder
{
    private readonly WijzigErkenningContext<ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpErkenningScenario> _ctx =
        new(new ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningOpvolgersWerdenToegevoegdAlsBeheerder.Beheerders.First());

    [Fact]
    public async ValueTask Then_Saves_ErkenningWerdGewijzigd()
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
                ErkenningStatus
                   .Bepaal(
                        ErkenningsPeriode.Create(command.Erkenning.StartDatum.Value, command.Erkenning.EindDatum.Value),
                        DateOnly.FromDateTime(DateTime.Today)
                    )
                   .Value,
                command.Erkenning.RedenVanWijziging
            )
        );
    }

    [Fact]
    public async ValueTask Then_OrganisatieBevoegdheidService_Not_Called()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.OrganisatieBevoegdheidService.VerifyNever();
    }
}
