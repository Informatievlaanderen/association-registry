namespace AssociationRegistry.Test.Dubbels.Allow_Loading_DubbeleVereniging;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.AddressMatch;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class When_Handling_TeAdresMatchenLocatieMessage : When_Loading_With_Dubbels_TestBase
{
    [Fact]
    public async Task Then_It_Should_Have_Loaded_AllowDubbels()
    {
        await VerifyVerenigingWasLoadedWithAllowDubbeleVereniging(async repositoryMock =>
        {
            var sut = new TeAdresMatchenLocatieMessageHandler(repositoryMock, Mock.Of<IGrarClient>(), NullLogger<TeAdresMatchenLocatieMessageHandler>.Instance);

            await sut.Handle(_fixture.Create<TeAdresMatchenLocatieMessage>() with
            {
                VCode = _fixture.Create<VCode>(),
            }, CancellationToken.None);
        });
    }
}
