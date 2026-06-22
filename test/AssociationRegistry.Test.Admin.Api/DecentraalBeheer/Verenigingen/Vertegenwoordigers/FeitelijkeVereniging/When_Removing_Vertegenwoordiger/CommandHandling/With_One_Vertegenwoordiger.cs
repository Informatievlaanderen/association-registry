namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Removing_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Xunit;

public class With_One_Vertegenwoordiger
{
    private readonly VerwijderVertegenwoordigerContext<FeitelijkeVerenigingWerdGeregistreerdWithOneVertegenwoordigerScenario> _ctx =
        new(new FeitelijkeVerenigingWerdGeregistreerdWithOneVertegenwoordigerScenario(),
            s => s.VertegenwoordigerId);

    [Fact]
    public async ValueTask Then_Throws_LaatsteVertegenwoordigerKanNietVerwijderdWordenException()
    {
        var command = _ctx.CreateCommand();

        await Assert.ThrowsAsync<LaatsteVertegenwoordigerKanNietVerwijderdWorden>(async () =>
            await _ctx.Handle(command)
        );
    }
}
