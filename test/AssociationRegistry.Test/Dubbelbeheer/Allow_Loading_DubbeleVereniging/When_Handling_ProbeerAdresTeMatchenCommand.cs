namespace AssociationRegistry.Test.Dubbelbeheer.Allow_Loading_DubbeleVereniging;

using AssociationRegistry.Grar.AdresMatch;
using AutoFixture;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using DecentraalBeheer.Vereniging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class When_Handling_ProbeerAdresTeMatchenCommand : When_Loading_With_Dubbels_TestBase
{
    [Fact]
    public async ValueTask Then_It_Should_Have_Loaded_AllowDubbels()
    {
        await VerifyVerenigingWasLoadedWithAllowDubbeleVereniging(async repositoryMock =>
        {
            var sut = new ProbeerAdresTeMatchenCommandHandler(repositoryMock, Mock.Of<IAdresMatchService>(), NullLogger<ProbeerAdresTeMatchenCommandHandler>.Instance);

            await sut.Handle(_fixture.Create<ProbeerAdresTeMatchenCommand>() with
            {
                VCode = _fixture.Create<VCode>(),
            }, CancellationToken.None);
        });
    }
}
