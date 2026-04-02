namespace AssociationRegistry.Test.Admin.AddressSync.When_Syncing_Locaties_Zonder_Adres_Match_With_LocatieId;

using AssociationRegistry.Admin.AddressSync;
using AssociationRegistry.Admin.AddressSync.Fetchers;
using AssociationRegistry.Admin.AddressSync.Handlers;
using AssociationRegistry.Admin.AddressSync.MessageHandling.Sqs.AddressSync;
using AssociationRegistry.Admin.Schema.Locaties;
using AssociationRegistry.Integrations.Grar.Integration.Messages;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
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
        var fetcher = new Mock<ITeSynchroniserenLocatiesZonderAdresMatchFetcher>();
        var handler = new Mock<IProbeerAdresTeMatchenCommandHandler>();

        fetcher
           .Setup(x => x.GetTeSynchroniserenLocatiesZonderAdresMatch(It.IsAny<IDocumentSession>(),
                                                                     It.IsAny<CancellationToken>()))
           .ReturnsAsync(fixture.CreateMany<LocatieZonderAdresMatchDocument>(5).ToArray());

        _capturedErrors = new List<AdressSyncError>();
        var callCount = 0;

        handler
           .Setup(x => x.Handle(It.IsAny<ProbeerAdresTeMatchenCommand>(), It.IsAny<CancellationToken>()))
           .Returns((ProbeerAdresTeMatchenCommand msg, CancellationToken _) =>
                {
                    callCount++;

                    if (callCount > 2)
                        return Task.CompletedTask;

                    var ex = new Exception(fixture.Create<string>());

                    _capturedErrors.Add(
                        new AdressSyncError(msg.VCode, [msg.LocatieId], ex)
                    );

                    return Task.FromException(ex);
                }
            );

        var sut = new SyncLocatieZonderAdresMatchProcessor(
            fetcher.Object,
            handler.Object,
            Mock.Of<IDocumentSession>(),
            NullLogger<SyncLocatieZonderAdresMatchProcessor>.Instance
        );

        _errors = sut.Process(CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_We_Return_AdresSyncErrors()
    {
        _errors.Should().BeEquivalentTo(_capturedErrors);
    }
}
