namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_Gemachtigde_Organisaties
{
    private readonly HefSchorsingErkenningOpContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario> _ctx =
        new(
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId
        );

    [Fact]
    public async ValueTask Then_Saves_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder_And_SchorsingWerdOpgeheven()
    {
        var command = _ctx.HefSchorsingErkenningOpCommand;

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
            new SchorsingVanErkenningWerdOpgeheven(command.ErkenningId, ErkenningStatus.Actief.Value),
            new VerenigingWerdErkend()
        );
    }
}
