namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Xunit;

public class Given_A_Duplicate_Vertegenwoordiger
{
    private readonly VoegVertegenwoordigerToeContext<FeitelijkeVerenigingWerdGeregistreerdScenario> _ctx =
        new(new FeitelijkeVerenigingWerdGeregistreerdScenario());

    [Fact]
    public async ValueTask Then_A_DuplicateVertegenwoordiger_Is_Thrown()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        await Assert.ThrowsAsync<InszMoetUniekZijn>(async () => await _ctx.Handle(command));
    }
}
