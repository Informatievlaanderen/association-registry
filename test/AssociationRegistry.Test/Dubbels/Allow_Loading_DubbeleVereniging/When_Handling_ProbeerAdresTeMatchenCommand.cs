namespace AssociationRegistry.Test.Dubbels.Allow_Loading_DubbeleVereniging;

using Acties.ProbeerAdresTeMatchen;
using AutoFixture;
using Grar;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging;
using Xunit;

public class When_Handling_ProbeerAdresTeMatchenCommand : When_Loading_With_Dubbels_TestBase
{
    [Fact]
    public async Task Then_It_Should_Have_Loaded_AllowDubbels()
    {
        await VerifyVerenigingWasLoadedWithAllowDubbeleVereniging(async repositoryMock =>
        {
            var sut = new ProbeerAdresTeMatchenCommandHandler(repositoryMock, Mock.Of<IGrarClient>(), NullLogger<ProbeerAdresTeMatchenCommandHandler>.Instance);

            await sut.Handle(_fixture.Create<ProbeerAdresTeMatchenCommand>() with
            {
                VCode = _fixture.Create<VCode>(),
            }, CancellationToken.None);
        });
    }
}
