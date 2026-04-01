namespace AssociationRegistry.Test.Admin.AddressSync.When_Syncing_Locaties_With_AdresId;

using AssociationRegistry.Admin.AddressSync.Fetchers;
using AssociationRegistry.Admin.AddressSync.Handlers;
using AssociationRegistry.Admin.AddressSync.MessageHandling.Sqs.AddressSync;
using AssociationRegistry.Integrations.Grar.Integration.Messages;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public class Given_Errors_Between_Handling_Messages
{
    private readonly string _exceptionMessage = "Timeout";
    private readonly Mock<ITeSynchroniserenLocatieAdresMessageHandler> _commandHandler;
    private const int MessageCount = 5;

    public Given_Errors_Between_Handling_Messages()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var fetcher = new Mock<ITeSynchroniserenLocatiesFetcher>();

        fetcher
            .Setup(x => x.GetTeSynchroniserenLocaties(It.IsAny<IDocumentSession>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.CreateMany<TeSynchroniserenLocatieAdresMessage>(MessageCount));

        _commandHandler = new Mock<ITeSynchroniserenLocatieAdresMessageHandler>();

        _commandHandler
            .SetupSequence(x =>
                x.Handle(It.IsAny<TeSynchroniserenLocatieAdresMessage>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask)
            .ThrowsAsync(new Exception(_exceptionMessage))
            .Returns(Task.CompletedTask)
            .ThrowsAsync(new Exception(_exceptionMessage))
            .Returns(Task.CompletedTask);

        var sut = new SyncLocatiesMetAdresIdProcessor(
            fetcher.Object,
            _commandHandler.Object,
            Mock.Of<IDocumentSession>(),
            NullLogger<SyncLocatiesMetAdresIdProcessor>.Instance
        );

        sut.Handle(CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_We_Still_Handle_All_The_Messages()
    {
        _commandHandler.Verify(
            x => x.Handle(It.IsAny<TeSynchroniserenLocatieAdresMessage>(), It.IsAny<CancellationToken>()),
            Times.Exactly(MessageCount)
        );
    }
}
