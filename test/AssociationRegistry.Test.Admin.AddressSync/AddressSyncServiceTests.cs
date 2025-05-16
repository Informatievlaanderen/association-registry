namespace AssociationRegistry.Test.Admin.AddressSync;

using AssociationRegistry.Admin.AddressSync;
using AssociationRegistry.Admin.AddressSync.Infrastructure.Notifications;
using AssociationRegistry.Admin.AddressSync.MessageHandling.Sqs.AddressSync;
using Grar;
using Marten;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Notifications;
using Vereniging;
using Wolverine;

public class AddressSyncServiceTests
{
    [Fact]
    public async ValueTask Given_TeSynchroniserenLocatiesFetcher_Throws_Then_A_Notification_Is_Sent()
    {
        var store = await TestDocumentStoreFactory.CreateAsync(nameof(AddressSyncServiceTests));
        var notifier = new Mock<INotifier>();

        var teSynchroniserenLocatiesFetcher = new Mock<ITeSynchroniserenLocatiesFetcher>();

        teSynchroniserenLocatiesFetcher.Setup(x => x.GetTeSynchroniserenLocaties(
                                                  It.IsAny<IDocumentSession>(),
                                                  It.IsAny<CancellationToken>()))
                                       .ThrowsAsync(new Exception());

        var addressSyncService = new AddressSyncService(store,
                                                        new TeSynchroniserenLocatieAdresMessageHandler(Mock.Of<IMessageBus>()),
                                                        teSynchroniserenLocatiesFetcher.Object,
                                                        notifier.Object,
                                                        NullLogger<AddressSyncService>.Instance,
                                                        new ApplicationLifetime(NullLogger<ApplicationLifetime>.Instance));

        await Assert.ThrowsAsync<Exception>(async () => await addressSyncService.StartAsync(CancellationToken.None));

        notifier.Verify(x => x.Notify(It.IsAny<AdresSynchronisatieGefaald>()));
    }
}
