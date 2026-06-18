namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Schors_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Verlopen_Erkenning
{
    private readonly SchorsErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask Then_Throw_VerlopenErkenningKanNietGeschorstWorden()
    {
        var command = _ctx.CreateCommand();

        var exception = await Assert.ThrowsAsync<VerlopenErkenningKanNietGeschorstWorden>(async () => await _ctx.Handle(command));

        exception.Message.Should().Be(ExceptionMessages.VerlopenErkenningKanNietGeschorstWorden);
    }
}
