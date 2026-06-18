namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Schors_Erkenning.CommandHandling.Vzer;

using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_ErkenningOpvolgersWerdenToegevoegdAlsBeheerder
{
    private readonly SchorsErkenningContext<ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpErkenningScenario> _ctx =
        new(new ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningOpvolgersWerdenToegevoegdAlsBeheerder.Beheerders.First());

    [Fact]
    public async ValueTask Then_Saves_ErkenningWerdGeschorst()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGeschorst(command.Erkenning.ErkenningId, command.Erkenning.RedenSchorsing)
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
