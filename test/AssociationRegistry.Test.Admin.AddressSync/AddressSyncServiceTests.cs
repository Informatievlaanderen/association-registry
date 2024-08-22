namespace AssociationRegistry.Test.Admin.AddressSync;

using AssociationRegistry.Admin.AddressSync;
using AssociationRegistry.Admin.AddressSync.Infrastructure.Notifications;
using Grar;
using Grar.AddressSync;
using Marten;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Notifications;
using Vereniging;

public class AddressSyncServiceTests
{
    [Fact]
    public async Task Given_TeSynchroniserenLocatiesFetcher_Throws_Then_A_Notification_Is_Sent()
    {
        var store = await TestDocumentStoreFactory.Create(nameof(AddressSyncServiceTests));
        var notifier = new Mock<INotifier>();

        var teSynchroniserenLocatiesFetcher = new Mock<ITeSynchroniserenLocatiesFetcher>();

        teSynchroniserenLocatiesFetcher.Setup(x => x.GetTeSynchroniserenLocaties(
                                                  It.IsAny<IDocumentSession>(),
                                                  It.IsAny<CancellationToken>()))
                                       .ThrowsAsync(new Exception());

        var addressSyncService = new AddressSyncService(store,
                                                        new TeSynchroniserenLocatieAdresMessageHandler(
                                                            Mock.Of<IVerenigingsRepository>(),
                                                            Mock.Of<IGrarClient>(),
                                                            NullLogger<TeSynchroniserenLocatieAdresMessageHandler>.Instance),
                                                        teSynchroniserenLocatiesFetcher.Object,
                                                        notifier.Object,
                                                        NullLogger<AddressSyncService>.Instance,
                                                        new ApplicationLifetime(NullLogger<ApplicationLifetime>.Instance));

        await Assert.ThrowsAsync<Exception>(async () => await addressSyncService.StartAsync(CancellationToken.None));

        notifier.Verify(x => x.Notify(It.IsAny<AdresSynchronisatieGefaald>()));
    }
}
