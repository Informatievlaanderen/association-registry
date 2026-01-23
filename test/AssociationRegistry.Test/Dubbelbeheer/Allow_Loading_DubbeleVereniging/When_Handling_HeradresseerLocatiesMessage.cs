namespace AssociationRegistry.Test.Dubbelbeheer.Allow_Loading_DubbeleVereniging;

using AssociationRegistry.Grar;
using AssociationRegistry.Integrations.Grar.Clients;
using AssociationRegistry.Vereniging;
using AutoFixture;
using CommandHandling.Grar.GrarConsumer.Messaging.HeradresseerLocaties;
using DecentraalBeheer.Vereniging;
using Moq;
using Xunit;

public class When_Handling_HeradresseerLocatiesMessage : When_Loading_With_Dubbels_TestBase
{
    [Fact]
    public async ValueTask Then_It_Should_Have_Loaded_AllowDubbels()
    {
        await VerifyVerenigingWasLoadedWithAllowDubbeleVereniging(async aggregateSession =>
        {
            var sut = new HeradresseerLocatiesMessageHandler(aggregateSession, Mock.Of<IGrarClient>());

            await sut.Handle(
                _fixture.Create<HeradresseerLocatiesMessage>() with
                {
                    VCode = _fixture.Create<VCode>(),
                },
                CancellationToken.None
            );
        });
    }
}
