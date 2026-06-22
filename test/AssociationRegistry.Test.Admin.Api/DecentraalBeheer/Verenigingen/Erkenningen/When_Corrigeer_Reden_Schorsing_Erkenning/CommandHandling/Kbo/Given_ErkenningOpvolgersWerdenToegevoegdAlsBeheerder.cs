namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Reden_Schorsing_Erkenning.CommandHandling.Kbo;

using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder
{
    private readonly CorrigeerRedenSchorsingErkenningContext<ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpGeschorsteErkenningScenario> _ctx =
        new(new ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpGeschorsteErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningOpvolgersWerdenToegevoegdAlsBeheerder.Beheerders.First());

    [Fact]
    public async ValueTask Then_Saves_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder_And_ErkenningRedenVanSchorsingWerdGecorrigeerd()
    {
        var command = _ctx.CorrigeerRedenSchorsingErkenningCommand;
        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningRedenVanSchorsingWerdGecorrigeerd(
                command.Erkenning.ErkenningId,
                command.Erkenning.RedenSchorsing
            )
        );
    }

    [Fact]
    public async ValueTask Then_OrganisatieBevoegdheidService_Not_Called()
    {
        var command = _ctx.CorrigeerRedenSchorsingErkenningCommand;
        await _ctx.Handle(command);

        _ctx.OrganisatieBevoegdheidService.VerifyNever();
    }
}
