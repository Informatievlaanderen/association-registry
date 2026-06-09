namespace AssociationRegistry.Test.Dubbelbeheer.Allow_Loading_DubbeleVereniging;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using AssociationRegistry.Framework;
using Xunit;

public class When_Handling_ActiveerErkenningCommand : When_Loading_With_Dubbels_TestBase
{
    [Fact]
    public async ValueTask Then_It_Should_Have_Loaded_AllowDubbels()
    {
        await VerifyVerenigingWasLoadedWithAllowDubbeleVereniging(async aggregateSession =>
        {
            var sut = new ActiveerErkenningCommandHandler(aggregateSession);
            var command = new ActiveerErkenningCommand("V0001001", 1);

            await sut.Handle(
                new CommandEnvelope<ActiveerErkenningCommand>(command, CommandMetadata.ForDigitaalVlaanderenProcess)
            );
        });
    }
}
