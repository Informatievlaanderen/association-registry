namespace AssociationRegistry.Test.KboSync.RecordProcessorTests;

using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using CommandHandling.KboSyncLambda.SyncKbo;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using CommandHandling.KboSyncLambda;
using Contracts.KboSync;
using Contracts.MagdaSync.KboSync;
using Integrations.Magda;
using Integrations.Slack;
using KboMutations.SyncLambda;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging;
using Wolverine;
using Xunit;

public class Given_KboSyncHandler_Return_A_Null_CommandResult
{
    [Fact(Skip = "We're going to trial not doing this anymore, and use metrics instead")]
    public async ValueTask Then_Nothing()
    {
        var fixture = new Fixture().CustomizeDomain();
        var logger = new Mock<ILogger>();
        var verenigingsRepository = new Mock<IVerenigingsRepository>();
        verenigingsRepository.Setup(x => x.Exists(It.IsAny<KboNummer>()))
                              .Returns(Task.FromResult(false));

        var syncKboCommandHandler = new SyncKboCommandHandler(Mock.Of<IMagdaRegistreerInschrijvingService>(), Mock.Of<IMagdaSyncGeefVerenigingService>(), Mock.Of<INotifier>(), NullLogger<SyncKboCommandHandler>.Instance);

        RecordProcessor.TryProcessRecord(logger.Object, verenigingsRepository.Object, CancellationToken.None,
                                                   new TeSynchroniserenKboNummerMessage(fixture.Create<KboNummer>()),
                                                   syncKboCommandHandler).GetAwaiter().GetResult();

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "No vereniging found for KBO number"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
