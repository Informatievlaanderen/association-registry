namespace AssociationRegistry.Test.Admin.AddressSync.When_Adressen_Sync;

using AssociationRegistry.Admin.AddressSync;
using AssociationRegistry.Admin.AddressSync.Handlers;
using AssociationRegistry.Admin.AddressSync.Infrastructure.Notifications;
using AutoFixture;
using Common.AutoFixture;
using Integrations.Slack;
using Marten;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public class Given_Errors_In_Processors
{
    private readonly AdressSyncError[] _capturedErrors;
    private readonly Mock<INotifier> _notifier;
    private readonly AddressSyncService _sut;
    private Mock<IHostApplicationLifetime> _hostApplication;

    public Given_Errors_In_Processors()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var syncLocatieAdresHandler = new Mock<ISyncLocatieAdresProcessor>();
        var syncLocatieZonderAdresMatchHandler = new Mock<ISyncLocatieZonderAdresMatchProcessor>();
        _hostApplication = new Mock<IHostApplicationLifetime>();

        var errorsMetAdres = fixture.CreateMany<AdressSyncError>(4).ToArray();
        var errorsZonderMatch = fixture.CreateMany<AdressSyncError>(3).ToArray();

        syncLocatieAdresHandler
           .Setup(x => x.Process(It.IsAny<CancellationToken>()))
           .ReturnsAsync(errorsMetAdres);

        syncLocatieZonderAdresMatchHandler
           .Setup(x => x.Process(It.IsAny<CancellationToken>()))
           .ReturnsAsync(errorsZonderMatch);

        _capturedErrors = errorsMetAdres.Concat(errorsZonderMatch).ToArray();

        _notifier = new Mock<INotifier>();

        _notifier
           .Setup(n => n.Notify(It.IsAny<INotification>()))
           .Returns(Task.CompletedTask);

        _sut = new AddressSyncService(
            syncLocatieAdresHandler.Object,
            syncLocatieZonderAdresMatchHandler.Object,
            Mock.Of<IDocumentSession>(),
            _hostApplication.Object,
            _notifier.Object,
            NullLogger<AddressSyncService>.Instance
        );

        _sut.SyncAdressen(CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_Notification_Is_Sent()
    {
        _notifier.Verify(
            x => x.Notify(It.IsAny<AdresSynchronisatieProcessorGefaald>()),
            Times.Once);
    }

    [Fact]
    public void Then_AdresSynchronisatieProcessorGefaald_Contains_Expected_Errors()
    {
        var expectedNotification = new AdresSynchronisatieProcessorGefaald(_capturedErrors);

        _notifier.Verify(
            x => x.Notify(
                It.Is<INotification>(n =>
                                         n != null
                                      && n.GetType() == typeof(AdresSynchronisatieProcessorGefaald)
                                      && ((AdresSynchronisatieProcessorGefaald)n).Value == expectedNotification.Value)),
            Times.Once);
    }

    [Fact]
    public void Then_Application_Is_Stopped()
    {
        _hostApplication.Verify(x => x.StopApplication(), Times.Once);
    }
}
