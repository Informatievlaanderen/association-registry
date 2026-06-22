namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Erkenning_Werd_Verlopen.CommandHandling.Vzer;

using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_Valid_Command
{
    private readonly VerloopErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithTeVerlopenErkenningScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithTeVerlopenErkenningScenario(),
            s => s.ErkenningWerdGeregistreerdTeVerlopen.ErkenningId,
            s => s.ErkenningWerdGeregistreerdTeVerlopen.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_It_Adds_An_ErkenningWerdVerlopen_Event()
    {
        var command = _ctx.VerloopErkenningCommand;

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(new ErkenningWerdVerlopen(command.ErkenningId));
    }
}
