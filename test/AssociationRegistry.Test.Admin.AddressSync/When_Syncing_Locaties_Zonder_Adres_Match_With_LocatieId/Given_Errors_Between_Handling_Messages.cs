namespace AssociationRegistry.Test.Admin.AddressSync.When_Syncing_Locaties_Zonder_Adres_Match_With_LocatieId;

using AssociationRegistry.Admin.AddressSync.Fetchers;
using AssociationRegistry.Admin.AddressSync.Handlers;
using AssociationRegistry.Admin.Schema.Locaties;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public class Given_Errors_Between_Handling_Messages
{
    private readonly string _exceptionMessage = "Timeout";
    private readonly Mock<IProbeerAdresTeMatchenCommandHandler> _commandHandler;
    private const int MessageCount = 5;

    public Given_Errors_Between_Handling_Messages()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var fetcher = new Mock<ITeSynchroniserenLocatiesZonderAdresMatchFetcher>();

        fetcher
            .Setup(x => x.GetTeSynchroniserenLocatiesZonderAdresMatch(It.IsAny<IDocumentSession>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.CreateMany<LocatieZonderAdresMatchDocument>(MessageCount).ToArray());

        _commandHandler = new Mock<IProbeerAdresTeMatchenCommandHandler>();

        _commandHandler
            .SetupSequence(x =>
                x.Handle(It.IsAny<ProbeerAdresTeMatchenCommand>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask)
            .ThrowsAsync(new Exception(_exceptionMessage))
            .Returns(Task.CompletedTask);

        var sut = new SyncLocatieZonderAdresMatchProcessor(
        fetcher.Object,
            _commandHandler.Object,
            Mock.Of<IDocumentSession>(),
            NullLogger<SyncLocatieZonderAdresMatchProcessor>.Instance
        );

        sut.Process(CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_We_Still_Handle_All_The_Messages()
    {
        _commandHandler.Verify(
            x => x.Handle(It.IsAny<ProbeerAdresTeMatchenCommand>(), It.IsAny<CancellationToken>()),
            Times.Exactly(15)
        );
    }
}
