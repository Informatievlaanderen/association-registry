namespace AssociationRegistry.Test.Admin.AddressSync.When_Syncing_Locaties_With_AdresId;

using AssociationRegistry.Admin.AddressSync.Fetchers;
using AssociationRegistry.Admin.AddressSync.Handlers;
using AssociationRegistry.Admin.AddressSync.MessageHandling.Sqs.AddressSync;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public class Given_No_Messages
{
    [Fact]
    public async ValueTask Given_No_Messages_No_Messages_Are_Sent()
    {
        var fetcher = new Mock<ITeSynchroniserenLocatiesFetcher>();
        var syncLocatieAdresHandler = new Mock<ITeSynchroniserenLocatieAdresMessageHandler>();

        fetcher
            .Setup(x => x.GetTeSynchroniserenLocaties(It.IsAny<IDocumentSession>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var sut = new SyncLocatiesMetAdresIdProcessor(
            fetcher.Object,
            syncLocatieAdresHandler.Object,
            Mock.Of<IDocumentSession>(),
            NullLogger<SyncLocatiesMetAdresIdProcessor>.Instance
        );

        var errors = await sut.Handle(CancellationToken.None);

        errors.Should().BeEmpty();
    }
}
