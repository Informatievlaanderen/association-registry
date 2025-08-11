namespace AssociationRegistry.Test.Admin.AddressSync;

using AssociationRegistry.Admin.AddressSync;
using AssociationRegistry.Admin.AddressSync.Infrastructure.Notifications;
using AssociationRegistry.Admin.AddressSync.MessageHandling.Sqs.AddressSync;
using Integrations.Slack;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;

public class AddressSyncServiceTests
{
    [Fact]
    public async ValueTask Given_TeSynchroniserenLocatiesFetcher_Throws_Then_A_Notification_Is_Sent()
    {
        var store = await TestDocumentStoreFactory.CreateAsync(nameof(AddressSyncServiceTests));

        var notifier = new Mock<INotifier>();
        var teSynchroniserenLocatiesFetcher = new Mock<ITeSynchroniserenLocatiesFetcher>();

        var handler = new Mock<TeSynchroniserenLocatieAdresMessageHandler>(
            Mock.Of<IMessageBus>()); // or substitute with a stubbed real instance

        // Setup the fetcher to throw
        teSynchroniserenLocatiesFetcher
           .Setup(x => x.GetTeSynchroniserenLocaties(It.IsAny<IDocumentSession>(), It.IsAny<CancellationToken>()))
           .ThrowsAsync(new Exception());

        // Setup scoped service provider
        var scopedServiceProvider = new Mock<IServiceProvider>();
        scopedServiceProvider.Setup(x => x.GetService(typeof(IDocumentStore))).Returns(store);
        scopedServiceProvider.Setup(x => x.GetService(typeof(INotifier))).Returns(notifier.Object);

        scopedServiceProvider.Setup(x => x.GetService(typeof(ITeSynchroniserenLocatiesFetcher)))
                             .Returns(teSynchroniserenLocatiesFetcher.Object);

        scopedServiceProvider.Setup(x => x.GetService(typeof(TeSynchroniserenLocatieAdresMessageHandler))).Returns(handler.Object);

        var serviceScope = new Mock<IServiceScope>();
        serviceScope.Setup(x => x.ServiceProvider).Returns(scopedServiceProvider.Object);

        var scopeFactory = new Mock<IServiceScopeFactory>();
        scopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);

        // Setup root service provider
        var rootServiceProvider = new Mock<IServiceProvider>();
        rootServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(scopeFactory.Object);

        var logger = NullLogger<AddressSyncService>.Instance;
        var lifetime = new ApplicationLifetime(NullLogger<ApplicationLifetime>.Instance);

        var addressSyncService = new AddressSyncService(rootServiceProvider.Object, logger, lifetime);

        await Assert.ThrowsAsync<Exception>(() => addressSyncService.StartAsync(CancellationToken.None));

        notifier.Verify(x => x.Notify(It.IsAny<AdresSynchronisatieGefaald>()), Times.Once);
    }
}
