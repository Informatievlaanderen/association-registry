namespace AssociationRegistry.Test.Admin.AddressSync.When_Syncing_Locaties_With_AdresId;

using AssociationRegistry.Admin.AddressSync;
using AssociationRegistry.Admin.AddressSync.Fetchers;
using AssociationRegistry.Admin.AddressSync.Handlers;
using AssociationRegistry.Admin.AddressSync.MessageHandling.Sqs.AddressSync;
using AssociationRegistry.Integrations.Grar.Integration.Messages;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public class Given_Exceptions_On_Handlers
{
    private AdressSyncError[] _errors;
    private List<AdressSyncError> _capturedErrors;

    public Given_Exceptions_On_Handlers()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var fetcher = new Mock<ITeSynchroniserenLocatiesFetcher>();
        var syncLocatieAdresHandler = new Mock<ITeSynchroniserenLocatieAdresMessageHandler>();

        fetcher
            .Setup(x => x.GetTeSynchroniserenLocaties(It.IsAny<IDocumentSession>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.CreateMany<TeSynchroniserenLocatieAdresMessage>(5));

        var commandHandler = new Mock<ITeSynchroniserenLocatieAdresMessageHandler>();

        _capturedErrors = new List<AdressSyncError>();
        var callCount = 0;

        syncLocatieAdresHandler
            .Setup(x => x.Handle(It.IsAny<TeSynchroniserenLocatieAdresMessage>(), It.IsAny<CancellationToken>()))
            .Returns(
                (TeSynchroniserenLocatieAdresMessage msg, CancellationToken _) =>
                {
                    callCount++;

                    if (callCount > 2)
                        return Task.CompletedTask;

                    var ex = new Exception(fixture.Create<string>());

                    _capturedErrors.Add(
                        new AdressSyncError(msg.VCode, msg.LocatiesWithAdres.Select(x => x.LocatieId).ToList(), ex)
                    );

                    return Task.FromException(ex);
                }
            );

        var sut = new SyncLocatiesMetAdresIdProcessor(
            fetcher.Object,
            syncLocatieAdresHandler.Object,
            Mock.Of<IDocumentSession>(),
            NullLogger<SyncLocatiesMetAdresIdProcessor>.Instance
        );

        _errors = sut.Handle(CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_We_Return_AdresSyncErrors()
    {
        _errors.Should().BeEquivalentTo(_capturedErrors);
    }
}
