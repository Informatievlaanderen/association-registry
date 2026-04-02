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

public class Given_Exception_In_Processors
{
    private readonly Mock<INotifier> _notifier;
    private readonly AddressSyncService _sut;
    private readonly string? _exceptionMessage = "Timeout";

    public Given_Exception_In_Processors()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var syncLocatieAdresHandler = new Mock<ISyncLocatieAdresProcessor>();
        var syncLocatieZonderAdresMatchHandler = new Mock<ISyncLocatieZonderAdresMatchProcessor>();

        var hostApplication = new Mock<IHostApplicationLifetime>();

        var errorsMetAdres = fixture.CreateMany<AdressSyncError>(4).ToArray();

        syncLocatieAdresHandler
           .Setup(x => x.Process(It.IsAny<CancellationToken>()))
           .ReturnsAsync(errorsMetAdres);

        syncLocatieZonderAdresMatchHandler
           .Setup(x => x.Process(It.IsAny<CancellationToken>()))
           .ThrowsAsync(new Exception(_exceptionMessage));

        _notifier = new Mock<INotifier>();

        _notifier
           .Setup(n => n.Notify(It.IsAny<INotification>()))
           .Returns(Task.CompletedTask);

        _sut = new AddressSyncService(
            syncLocatieAdresHandler.Object,
            syncLocatieZonderAdresMatchHandler.Object,
            Mock.Of<IDocumentSession>(),
            hostApplication.Object,
            _notifier.Object,
            NullLogger<AddressSyncService>.Instance
        );
    }

    [Fact]
    public async Task Then_Throws_Exception()
    {
        await Assert.ThrowsAsync<Exception>(() => _sut.SyncAdressen(CancellationToken.None));
    }

    [Fact]
    public async Task Then_Notification_Is_Sent()
    {
        await Assert.ThrowsAsync<Exception>(() => _sut.SyncAdressen(CancellationToken.None));

        _notifier.Verify(
            x => x.Notify(It.IsAny<AdresSynchronisatieGefaald>()),
            Times.Once);
    }
}
