namespace AssociationRegistry.Test.Admin.AddressSync.When_Syncing_Locaties_Zonder_Adres_Match_With_LocatieId;

using AssociationRegistry.Admin.AddressSync.Fetchers;
using AssociationRegistry.Admin.AddressSync.Handlers;
using AssociationRegistry.Admin.AddressSync.MessageHandling.Sqs.AddressSync;
using CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public class Given_No_Messages
{
    [Fact]
    public async ValueTask Given_No_Messages_No_Messages_Are_Sent()
    {
        var fetcher = new Mock<ITeSynchroniserenLocatiesZonderAdresMatchFetcher>();
        var handler = new Mock<IProbeerAdresTeMatchenCommandHandler>();

        fetcher
            .Setup(x => x.GetTeSynchroniserenLocatiesZonderAdresMatch(It.IsAny<IDocumentSession>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var sut = new SyncLocatieZonderAdresMatchProcessor(
            fetcher.Object,
            handler.Object,
            Mock.Of<IDocumentSession>(),
            NullLogger<SyncLocatieZonderAdresMatchProcessor>.Instance
        );

        var errors = await sut.Process(CancellationToken.None);

        errors.Should().BeEmpty();
    }
}
