namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Xunit;

public class Given_A_Second_Primair_Vertegenwoordiger
{
    private readonly VoegVertegenwoordigerToeContext<FeitelijkeVerenigingWerdGeregistreerdScenario> _ctx =
        new(new FeitelijkeVerenigingWerdGeregistreerdScenario());

    [Fact]
    public async ValueTask Then_A_DuplicateVertegenwoordiger_Is_Thrown()
    {
        await _ctx.Handle(_ctx.CreatePrimairCommand());

        await Assert.ThrowsAsync<MeerderePrimaireVertegenwoordigers>(async () =>
            await _ctx.Handle(_ctx.CreatePrimairCommand()));
    }
}
