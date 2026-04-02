namespace AssociationRegistry.Test.Admin.AddressSync.When_Adressen_Sync;

using AssociationRegistry.Admin.AddressSync;
using AssociationRegistry.Admin.AddressSync.Handlers;
using Integrations.Slack;
using Marten;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public class Given_No_Errors_In_Processors
{
    private readonly Mock<INotifier> _notifier;
    private readonly AddressSyncService _sut;
    private Mock<IHostApplicationLifetime> _hostApplication;
    private Mock<ISyncLocatieAdresProcessor> _syncLocatieAdresProcessor;
    private Mock<ISyncLocatieZonderAdresMatchProcessor> _syncLocatieZonderAdresMatchProcessor;

    public Given_No_Errors_In_Processors()
    {
        _syncLocatieAdresProcessor = new Mock<ISyncLocatieAdresProcessor>();
        _syncLocatieZonderAdresMatchProcessor = new Mock<ISyncLocatieZonderAdresMatchProcessor>();
        _hostApplication = new Mock<IHostApplicationLifetime>();

        _syncLocatieAdresProcessor
           .Setup(x => x.Process(It.IsAny<CancellationToken>()))
           .ReturnsAsync([]);

        _syncLocatieZonderAdresMatchProcessor
           .Setup(x => x.Process(It.IsAny<CancellationToken>()))
           .ReturnsAsync([]);

        _notifier = new Mock<INotifier>();

        _notifier
           .Setup(n => n.Notify(It.IsAny<INotification>()))
           .Returns(Task.CompletedTask);

        _sut = new AddressSyncService(
            _syncLocatieAdresProcessor.Object,
            _syncLocatieZonderAdresMatchProcessor.Object,
            Mock.Of<IDocumentSession>(),
            _hostApplication.Object,
            _notifier.Object,
            NullLogger<AddressSyncService>.Instance
        );

        _sut.SyncAdressen(CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_Notifier_Has_Notified()
    {
        _notifier.Verify(
            x => x.Notify(It.IsAny<INotification>()),
            Times.Never);
    }

    [Fact]
    public void Then_Application_Is_Stopped()
    {
        _hostApplication.Verify(
            x => x.StopApplication(),
            Times.Once);
    }

    [Fact]
    public void Then_SyncLocatieAdresProcessor_Is_Executed()
    {
        _syncLocatieAdresProcessor.Verify(
            x => x.Process(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void Then_SyncLocatieZonderAdresMatchProcessor_Is_Executed()
    {
        _syncLocatieZonderAdresMatchProcessor.Verify(
            x => x.Process(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
