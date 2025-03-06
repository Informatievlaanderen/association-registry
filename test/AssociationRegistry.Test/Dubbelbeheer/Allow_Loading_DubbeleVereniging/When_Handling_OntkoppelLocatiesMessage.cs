namespace AssociationRegistry.Test.Dubbelbeheer.Allow_Loading_DubbeleVereniging;

using AssociationRegistry.Vereniging;
using AutoFixture;
using Grar.GrarConsumer.Messaging.OntkoppelAdres;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class When_Handling_OntkoppelLocatiesMessage : When_Loading_With_Dubbels_TestBase
{
    [Fact]
    public async Task Then_It_Should_Have_Loaded_AllowDubbels()
    {
        await VerifyVerenigingWasLoadedWithAllowDubbeleVereniging(async repositoryMock =>
        {
            var sut = new OntkoppelLocatiesMessageHandler(repositoryMock);

            await sut.Handle(_fixture.Create<OntkoppelLocatiesMessage>() with
            {
                VCode = _fixture.Create<VCode>(),
            }, CancellationToken.None);
        });
    }
}
