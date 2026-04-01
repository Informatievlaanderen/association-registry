namespace AssociationRegistry.Test.Admin.AddressSync.Handlers;

using AssociationRegistry.Admin.AddressSync;
using AssociationRegistry.Admin.AddressSync.Fetchers;
using AssociationRegistry.Admin.AddressSync.Handlers;
using AssociationRegistry.Admin.AddressSync.MessageHandling.Sqs.AddressSync;
using AssociationRegistry.Grar.Models;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Integrations.Grar.Integration.Messages;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;

public class SyncLocatieAdresHandlerTests
{
    [Fact]
    public async ValueTask Given_No_Messages_Returns_Empty_Errors()
    {
        var fetcher = new Mock<ITeSynchroniserenLocatiesFetcher>();
        var syncLocatieAdresHandler = new Mock<ITeSynchroniserenLocatieAdresMessageHandler>();

        fetcher
           .Setup(x => x.GetTeSynchroniserenLocaties(
                      It.IsAny<IDocumentSession>(),
                      It.IsAny<CancellationToken>()))
           .ReturnsAsync(Array.Empty<TeSynchroniserenLocatieAdresMessage>());

        var handler = new SyncLocatieAdresHandler(
            NullLogger<SyncLocatieAdresHandler>.Instance,
            fetcher.Object,
            syncLocatieAdresHandler.Object);

        var errors = await handler.Handle(Mock.Of<IDocumentSession>(), CancellationToken.None);
        Assert.Empty(errors);
    }

    [Fact]
    public async ValueTask Given_Multiple_Messages_With_Spread_Exceptions_Returns_Correct_Errors()
    {
        var store = await TestDocumentStoreFactory.CreateAsync(nameof(SyncLocatieAdresHandlerTests));
        await using var session = store.LightweightSession();
        var fetcher = new Mock<ITeSynchroniserenLocatiesFetcher>();
        var fixture = new Fixture().CustomizeAdminApi();
        var message1 = fixture.Create<TeSynchroniserenLocatieAdresMessage>();
        var message2 = fixture.Create<TeSynchroniserenLocatieAdresMessage>();
        var message3 = fixture.Create<TeSynchroniserenLocatieAdresMessage>();
        var message4 = fixture.Create<TeSynchroniserenLocatieAdresMessage>();
        var message5 = fixture.Create<TeSynchroniserenLocatieAdresMessage>();
        var commandHandler = new Mock<ITeSynchroniserenLocatieAdresMessageHandler>();
        var syncLocatieAdresHandler = new Mock<ITeSynchroniserenLocatieAdresMessageHandler>();

        var callIndex = 0;

        commandHandler
           .Setup(x => x.Handle(It.IsAny<TeSynchroniserenLocatieAdresMessage>(), It.IsAny<CancellationToken>()))
           .Returns<TeSynchroniserenLocatieAdresMessage, CancellationToken>((msg, _) =>
            {
                var currentIndex = callIndex++;

                if (currentIndex == 3 || currentIndex == 4)
                    throw new Exception();

                return Task.CompletedTask;
            });

        fetcher
           .Setup(x => x.GetTeSynchroniserenLocaties(
                      It.IsAny<IDocumentSession>(),
                      It.IsAny<CancellationToken>()))
           .ReturnsAsync([message1, message2, message3, message4, message5]);

        var handler = new SyncLocatieAdresHandler(NullLogger<SyncLocatieAdresHandler>.Instance, fetcher.Object, syncLocatieAdresHandler.Object);

        var errors = await handler.Handle(session, CancellationToken.None);

        errors.Length.Should().Be(2);

        errors.Should().BeEquivalentTo(
            new[]
            {
                new AdressSyncError(message3.VCode, message3.LocatiesWithAdres.Select(x => x.LocatieId).ToList()),
                new AdressSyncError(message4.VCode, message4.LocatiesWithAdres.Select(x => x.LocatieId).ToList()),
            });

        commandHandler.Verify(
            x => x.Handle(It.IsAny<TeSynchroniserenLocatieAdresMessage>(), It.IsAny<CancellationToken>()),
            Times.Exactly(5));
    }
}
