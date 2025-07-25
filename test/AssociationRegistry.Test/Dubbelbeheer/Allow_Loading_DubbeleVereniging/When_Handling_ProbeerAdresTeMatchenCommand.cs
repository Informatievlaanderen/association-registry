﻿namespace AssociationRegistry.Test.Dubbelbeheer.Allow_Loading_DubbeleVereniging;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Administratie.ProbeerAdresTeMatchen;
using DecentraalBeheer.Locaties.ProbeerAdresTeMatchen;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging.Geotags;
using Xunit;

public class When_Handling_ProbeerAdresTeMatchenCommand : When_Loading_With_Dubbels_TestBase
{
    [Fact]
    public async ValueTask Then_It_Should_Have_Loaded_AllowDubbels()
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
