namespace AssociationRegistry.Test.Dubbelbeheer.Allow_Loading_DubbeleVereniging;

using AssociationRegistry.Acties.GrarConsumer.HeradresseerLocaties;
using AssociationRegistry.Grar;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Moq;
using Xunit;

public class When_Handling_HeradresseerLocatiesMessage : When_Loading_With_Dubbels_TestBase
{
    [Fact]
    public async Task Then_It_Should_Have_Loaded_AllowDubbels()
    {
        await VerifyVerenigingWasLoadedWithAllowDubbeleVereniging(async repositoryMock =>
        {
            var sut = new HeradresseerLocatiesMessageHandler(repositoryMock, Mock.Of<IGrarClient>());

            await sut.Handle(_fixture.Create<HeradresseerLocatiesMessage>() with
            {
                VCode = _fixture.Create<VCode>(),
            }, CancellationToken.None);
        });
    }
}
