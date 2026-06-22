namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Reden_Schorsing_Erkenning.CommandHandling.Vzer;

using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.Wegwijs;
using Events;
using Xunit;

public class Given_Gemachtigde_Organisaties
{
    private readonly CorrigeerRedenSchorsingErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId);

    [Fact]
    public async ValueTask Then_Saves_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder_And_ErkenningRedenVanSchorsingWerdGecorrigeerd()
    {
        var command = _ctx.CorrigeerRedenSchorsingErkenningCommand;
        var organisatieBevoegdheidService = new IOrganisatieBevoegdheidServiceMockStub().WithGemachtigdeOrganisaties([
            _ctx.Metadata.Initiator,
        ]);
        await _ctx.Handle(command, service: organisatieBevoegdheidService.Object);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningOpvolgersWerdenToegevoegdAlsBeheerder(
                _ctx.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                [_ctx.Metadata.Initiator]
            ),
            new ErkenningRedenVanSchorsingWerdGecorrigeerd(
                command.Erkenning.ErkenningId,
                command.Erkenning.RedenSchorsing
            )
        );
    }
}
