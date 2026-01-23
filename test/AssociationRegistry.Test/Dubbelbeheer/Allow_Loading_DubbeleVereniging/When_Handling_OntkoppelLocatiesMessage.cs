namespace AssociationRegistry.Test.Dubbelbeheer.Allow_Loading_DubbeleVereniging;

using AssociationRegistry.Vereniging;
using AutoFixture;
using CommandHandling.Grar.GrarConsumer.Messaging.OntkoppelAdres;
using DecentraalBeheer.Vereniging;
using Xunit;

public class When_Handling_OntkoppelLocatiesMessage : When_Loading_With_Dubbels_TestBase
{
    [Fact]
    public async ValueTask Then_It_Should_Have_Loaded_AllowDubbels()
    {
        await VerifyVerenigingWasLoadedWithAllowDubbeleVereniging(async aggregateSession =>
        {
            var sut = new OntkoppelLocatiesMessageHandler(aggregateSession);

            await sut.Handle(
                _fixture.Create<OntkoppelLocatiesMessage>() with
                {
                    VCode = _fixture.Create<VCode>(),
                },
                CancellationToken.None
            );
        });
    }
}
