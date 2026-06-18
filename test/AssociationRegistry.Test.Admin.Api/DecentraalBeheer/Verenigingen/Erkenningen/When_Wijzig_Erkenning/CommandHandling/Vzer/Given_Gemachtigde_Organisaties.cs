namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_Gemachtigde_Organisaties
{
    private readonly WijzigErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId);

    [Fact]
    public async ValueTask Then_Saves_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder_And_ErkenningWerdGewijzigd()
    {
        var command = _ctx.CreateCommand();
        var service = _ctx.OrganisatieBevoegdheidService.WithGemachtigdeOrganisaties([_ctx.Metadata.Initiator]);

        await _ctx.Handle(command, service: service.Object);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningOpvolgersWerdenToegevoegdAlsBeheerder(
                _ctx.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                [_ctx.Metadata.Initiator]
            ),
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
}
