namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Verwijder_Erkenning.CommandHandling.Kbo;

using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_Gemachtigde_Organisaties
{
    private readonly VerwijderErkenningContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId
        );

    [Fact]
    public async ValueTask Then_Saves_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder_And_ErkenningWerdVerwijderd()
    {
        var command = _ctx.VerwijderErkenningCommand;
        var service = _ctx.OrganisatieBevoegdheidService.WithGemachtigdeOrganisaties(
            _ctx.Scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
            [_ctx.Metadata.Initiator]
        );

        await _ctx.Handle(command, service: service.Object);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningOpvolgersWerdenToegevoegdAlsBeheerder(
                _ctx.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                [_ctx.Metadata.Initiator]
            ),
            new ErkenningWerdVerwijderd(command.ErkenningId)
        );
    }
}
