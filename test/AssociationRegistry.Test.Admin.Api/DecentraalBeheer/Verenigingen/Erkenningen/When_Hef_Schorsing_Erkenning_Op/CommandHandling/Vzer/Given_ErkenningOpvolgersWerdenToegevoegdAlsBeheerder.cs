namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder
{
    private readonly HefSchorsingErkenningOpContext<ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpGeschorsteErkenningScenario> _ctx =
        new(
            new ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpGeschorsteErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningOpvolgersWerdenToegevoegdAlsBeheerder.Beheerders.First()
        );

    [Fact]
    public async ValueTask Then_Saves_SchorsingVanErkenningWerdOpgeheven()
    {
        var command = _ctx.HefSchorsingErkenningOpCommand;

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new SchorsingVanErkenningWerdOpgeheven(command.ErkenningId, ErkenningStatus.Actief.Value),
            new VerenigingWerdErkend()
        );
    }

    [Fact]
    public async ValueTask Then_OrganisatieBevoegdheidService_Not_Called()
    {
        var command = _ctx.HefSchorsingErkenningOpCommand;

        await _ctx.Handle(command);

        _ctx.OrganisatieBevoegdheidService.VerifyNever();
    }
}
