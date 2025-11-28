namespace AssociationRegistry.Test.Admin.Api.Bewaartermijnen.When_Sending_A_VerwijderPersoonsgegevens_Message;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.Bewaartermijnen.Reacties.VerwijderVertegenwoordigerPersoonsgegevens;
using Common.AutoFixture;
using Events;
using Marten;
using MartenDb.Store;
using Moq;
using NodaTime;
using Persoonsgegevens;
using Xunit;

public class With_Valid_Message_VerwijderVertegenwoordigerPersoonsgegevensMessage
{
    private Mock<IEventStore> _eventStore;
    private readonly VCode _vCode;
    private readonly int _vertegenwoordigerId;
    private CommandMetadata _commandMetadata;
    private string _bewaartermijnId;
    private readonly Instant _vervalDag;
    private readonly Mock<IVertegenwoordigerPersoonsgegevensRepository> _vertegenwoordigerPersoonsgegevensRepository;
    private readonly  Mock<IDocumentSession> _session;

    public With_Valid_Message_VerwijderVertegenwoordigerPersoonsgegevensMessage()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        _vCode = fixture.Create<VCode>();
        _vertegenwoordigerId = fixture.Create<int>();
        _bewaartermijnId = $"{_vCode}-{_vertegenwoordigerId}";
        _vervalDag = fixture.Create<Instant>();
        var message = new VerwijderVertegenwoordigerPersoonsgegevensMessage(_bewaartermijnId, _vCode, _vertegenwoordigerId, _vervalDag);
        _commandMetadata = CommandMetadata.ForDigitaalVlaanderenProcess;
        var commandHandler = new VerwijderVertegenwoordigerPersoonsgegevensMessageHandler();
        _eventStore = new Mock<IEventStore>();
        _vertegenwoordigerPersoonsgegevensRepository = new Mock<IVertegenwoordigerPersoonsgegevensRepository>();
        _session = new Mock<IDocumentSession>();

        commandHandler.Handle(message, _session.Object, _eventStore.Object, _vertegenwoordigerPersoonsgegevensRepository.Object, CancellationToken.None)
                      .GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_BewaartermijnWerdVerlopen_Is_Saved_On_Bewaartermijn()
    {

        _eventStore.Verify(x => x.SaveTransactional(
                               _bewaartermijnId,
                               null,
                               It.Is<CommandMetadata>(x => x.Initiator == _commandMetadata.Initiator),
                               It.IsAny<CancellationToken>(),
                               It.Is<IEvent[]>(events =>
                                                                     events.Length == 1 &&
                                                                     ((BewaartermijnWerdVerlopen)events[0]).BewaartermijnId == _bewaartermijnId &&
                                                                     ((BewaartermijnWerdVerlopen)events[0]).VCode == _vCode.ToString() &&
                                                                     ((BewaartermijnWerdVerlopen)events[0]).VertegenwoordigerId == _vertegenwoordigerId &&
                                                                     ((BewaartermijnWerdVerlopen)events[0]).Vervaldag == _vervalDag
                               )), Times.Once());
    }

    [Fact]
    public void Then_BewaartermijnWerdVerlopen_Is_Saved_On_Vereniging()
    {

        _eventStore.Verify(x => x.SaveTransactional(
                               _vCode,
                               null,
                               It.Is<CommandMetadata>(x => x.Initiator == _commandMetadata.Initiator),
                               It.IsAny<CancellationToken>(),
                               It.Is<IEvent[]>(events =>
                                                                     events.Length == 1 &&
                                                                     ((BewaartermijnWerdVerlopen)events[0]).BewaartermijnId == _bewaartermijnId &&
                                                                     ((BewaartermijnWerdVerlopen)events[0]).VCode == _vCode.ToString() &&
                                                                     ((BewaartermijnWerdVerlopen)events[0]).VertegenwoordigerId == _vertegenwoordigerId &&
                                                                     ((BewaartermijnWerdVerlopen)events[0]).Vervaldag == _vervalDag
                               )),
                               Times.Once());
    }

    [Fact]
    public void Then_VertegenwoordigerPersoonsgegevens_Are_Deleted()
    {
        _vertegenwoordigerPersoonsgegevensRepository.Verify(x => x.Delete(_vCode, _vertegenwoordigerId));
    }

    [Fact]
    public void Then_Session_Is_Saved()
    {
        _session.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }
}
