namespace AssociationRegistry.Test.Dubbelbeheer.Allow_Loading_DubbeleVereniging;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerloopErkenning;
using AssociationRegistry.Framework;
using Xunit;

public class When_Handling_VerloopErkenningCommand : When_Loading_With_Dubbels_TestBase
{
    [Fact]
    public async ValueTask Then_It_Should_Have_Loaded_AllowDubbels()
    {
        await VerifyVerenigingWasLoadedWithAllowDubbeleVereniging(async aggregateSession =>
        {
            var sut = new VerloopErkenningCommandHandler(aggregateSession);
            var command = new VerloopErkenningCommand("V0001001", 1);

            await sut.Handle(
                new CommandEnvelope<VerloopErkenningCommand>(command, CommandMetadata.ForDigitaalVlaanderenProcess)
            );
        });
    }
}
