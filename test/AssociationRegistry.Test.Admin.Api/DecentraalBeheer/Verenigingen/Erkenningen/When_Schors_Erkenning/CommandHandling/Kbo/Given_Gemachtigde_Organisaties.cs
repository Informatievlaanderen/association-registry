namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Schors_Erkenning.CommandHandling.Kbo;

using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_Gemachtigde_Organisaties
{
    private readonly SchorsErkenningContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId
        );

    [Fact]
    public async ValueTask Then_Saves_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder_And_ErkenningWerdGeschorst()
    {
        var command = _ctx.SchorsErkenningCommand;
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
            new ErkenningWerdGeschorst(command.Erkenning.ErkenningId, command.Erkenning.RedenSchorsing)
        );
    }
}
