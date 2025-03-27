﻿namespace AssociationRegistry.Test.KboSync.RecordProcessorTests;

using AutoFixture;
using Common.AutoFixture;
using Kbo;
using KboSyncLambda;
using KboSyncLambda.SyncKbo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Notifications;
using Vereniging;
using Xunit;

public class Given_KboSyncHandler_Return_A_Null_CommandResult
{
    [Fact]
    public async Task Then_Nothing()
    {
        var fixture = new Fixture().CustomizeDomain();
        var logger = new Mock<ILogger>();
        var verenigingsRepository = new Mock<IVerenigingsRepository>();
        verenigingsRepository.Setup(x => x.Exists(It.IsAny<KboNummer>()))
                              .Returns(Task.FromResult(false));

        var syncKboCommandHandler = new SyncKboCommandHandler(Mock.Of<IMagdaRegistreerInschrijvingService>(), Mock.Of<IMagdaGeefVerenigingService>(), Mock.Of<INotifier>(), NullLogger<SyncKboCommandHandler>.Instance);

        RecordProcessor.TryProcessRecord(logger.Object, verenigingsRepository.Object, CancellationToken.None,
                                                   new TeSynchroniserenKboNummerMessage(fixture.Create<KboNummer>()),
                                                   syncKboCommandHandler).GetAwaiter().GetResult();

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "Sync resulted in nothing to sync."),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
