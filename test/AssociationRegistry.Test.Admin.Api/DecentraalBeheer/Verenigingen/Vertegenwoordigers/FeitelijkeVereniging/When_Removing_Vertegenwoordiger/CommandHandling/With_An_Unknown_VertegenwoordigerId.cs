namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Removing_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Xunit;

public class With_An_Unknown_VertegenwoordigerId
{
    private readonly VerwijderVertegenwoordigerContext<FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields> _ctx =
        new(new FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields(),
            _ => 0);

    [Fact]
    public async ValueTask Then_A_UnknownVertegenoordigerException_Is_Thrown()
    {
        var command = _ctx.CreateCommand(vertegenwoordigerId: _ctx.CreateUnknownVertegenwoordigerId());

        await Assert.ThrowsAsync<VertegenwoordigerIsNietGekend>(async () => await _ctx.Handle(command));
    }
}
