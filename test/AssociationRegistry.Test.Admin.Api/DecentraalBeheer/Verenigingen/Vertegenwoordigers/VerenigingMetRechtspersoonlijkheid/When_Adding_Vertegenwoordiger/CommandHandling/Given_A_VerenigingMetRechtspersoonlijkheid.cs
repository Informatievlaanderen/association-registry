namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingMetRechtspersoonlijkheid.When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FeitelijkeVereniging.When_Adding_Vertegenwoordiger.CommandHandling;
using Xunit;

public class Given_A_VerenigingMetRechtspersoonlijkheid
{
    private readonly VoegVertegenwoordigerToeContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario());

    [Fact]
    public async ValueTask Then_A_UnsupportedOperationException_Is_Thrown()
    {
        var command = _ctx.CreateCommand();

        await Assert.ThrowsAsync<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersToevoegen>(async () =>
            await _ctx.Handle(command)
        );
    }
}
