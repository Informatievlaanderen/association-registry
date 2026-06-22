namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Removing_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Xunit;

public class With_One_Vertegenwoordiger_And_An_Unknown_VertegenwoordigerId
{
    private readonly VerwijderVertegenwoordigerContext<FeitelijkeVerenigingWerdGeregistreerdWithOneVertegenwoordigerScenario> _ctx =
        new(new FeitelijkeVerenigingWerdGeregistreerdWithOneVertegenwoordigerScenario(),
            s => s.VertegenwoordigerId);

    [Fact]
    public async ValueTask Then_A_UnknownVertegenoordigerException_Is_Thrown()
    {
        var command = _ctx.CreateCommand(vertegenwoordigerId: _ctx.CreateUnknownVertegenwoordigerId());

        await Assert.ThrowsAsync<VertegenwoordigerIsNietGekend>(async () => await _ctx.Handle(command));
    }
}
