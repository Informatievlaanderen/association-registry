namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Schors_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Verlopen_Erkenning
{
    private readonly SchorsErkenningContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_Throw_VerlopenErkenningKanNietGeschorstWorden()
    {
        var command = _ctx.CreateCommand();

        var exception = await Assert.ThrowsAsync<VerlopenErkenningKanNietGeschorstWorden>(async () => await _ctx.Handle(command));

        exception.Message.Should().Be(string.Format(ExceptionMessages.VerlopenErkenningKanNietGeschorstWorden));
    }
}
