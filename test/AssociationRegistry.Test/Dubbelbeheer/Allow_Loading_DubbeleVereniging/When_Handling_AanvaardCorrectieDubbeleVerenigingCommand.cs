namespace AssociationRegistry.Test.Dubbelbeheer.Allow_Loading_DubbeleVereniging;

using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardCorrectieDubbel;
using Xunit;

public class When_Handling_AanvaardCorrectieDubbeleVerenigingCommand : When_Loading_With_Dubbels_TestBase
{
    [Fact]
    public async ValueTask Then_It_Should_Have_Loaded_AllowDubbels()
    {
        await VerifyVerenigingWasLoadedWithAllowDubbeleVereniging(async aggregateSession =>
        {
            var sut = new AanvaardCorrectieDubbeleVerenigingCommandHandler(aggregateSession);

            await sut.Handle(_fixture.Create<AanvaardCorrectieDubbeleVerenigingCommand>(), CancellationToken.None);
        });
    }
}
