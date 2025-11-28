namespace AssociationRegistry.Test.Admin.Api.Bewaartermijnen.When_Starting_A_Bewaartermijn;

using AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using CommandHandling.Bewaartermijnen;
using Events;
using Integrations.Grar.Bewaartermijnen;
using MartenDb.Store;
using Moq;
using Wolverine;
using Xunit;

public class With_Valid_Message
{
    private Mock<IEventStore> _eventStore;
    private readonly VCode _vCode;
    private readonly int _vertegenwoordigerId;
    private CommandMetadata _commandMetadata;
    private BewaartermijnOptions _bewaartermijnOptions;

    public With_Valid_Message()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        _vCode = fixture.Create<VCode>();
        _vertegenwoordigerId = fixture.Create<int>();
        var command = new StartBewaartermijnMessage(_vCode, _vertegenwoordigerId);
        _commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new StartBewaartermijnMessageHandler();
        _eventStore = new Mock<IEventStore>();
        _bewaartermijnOptions = new BewaartermijnOptions(){
            Duration = TimeSpan.FromDays(1),
        };

        commandHandler.Handle(new CommandEnvelope<StartBewaartermijnMessage>(command, _commandMetadata), _eventStore.Object, Mock.Of<IMessageBus>(), _bewaartermijnOptions, CancellationToken.None)
                      .GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Bewaartermijn_Is_Saved()
    {
        var expectedAggregateId = $"{_vCode}-{_vertegenwoordigerId}";
        var expectedVervaldag = _commandMetadata.Tijdstip.PlusTicks(_bewaartermijnOptions.Duration.Ticks);
        var expectedEvent = new BewaartermijnWerdGestart(expectedAggregateId, _vCode.ToString(), _vertegenwoordigerId,
                                                         expectedVervaldag);
        _eventStore.Verify(x => x.SaveNew(
                               expectedAggregateId,
                               _commandMetadata,
                               It.IsAny<CancellationToken>(),
                               It.Is<IEvent[]>(events =>
                                                                     events.Length == 1 &&
                                                                     ((BewaartermijnWerdGestart)events[0]).BewaartermijnId == expectedAggregateId &&
                                                                     ((BewaartermijnWerdGestart)events[0]).VCode == _vCode.ToString() &&
                                                                     ((BewaartermijnWerdGestart)events[0]).VertegenwoordigerId == _vertegenwoordigerId &&
                                                                     ((BewaartermijnWerdGestart)events[0]).Vervaldag == expectedVervaldag
                               )));
    }
}
