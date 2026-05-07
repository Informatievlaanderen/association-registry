namespace AssociationRegistry.Test.Admin.Api.Bewaartermijnen.When_Starting_A_Bewaartermijn.Given_A_StartBewaartermijnCommand;

using AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Grar.Bewaartermijnen;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Integrations.Slack;
using Moq;
using NodaTime;
using Xunit;

public class With_Valid_Message
{
    private Mock<IEventStore> _eventStore;
    private Mock<INotifier> _notifier;
    private readonly VCode _vCode;
    private readonly int _entityId;
    private CommandMetadata _commandMetadata;
    private BewaartermijnOptions _bewaartermijnOptions;
    private Instant _expectedVervaldag;

    public With_Valid_Message()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        _vCode = fixture.Create<VCode>();
        _entityId = fixture.Create<int>();
        _commandMetadata = CommandMetadata.ForDigitaalVlaanderenProcess;

        _bewaartermijnOptions = new BewaartermijnOptions() { Duration = TimeSpan.FromDays(1) };

        _expectedVervaldag = _commandMetadata.Tijdstip.PlusTicks(_bewaartermijnOptions.Duration.Ticks);

        var message = new StartBewaartermijnCommand(
            _vCode,
            PersoonsgegevensType.Vertegenwoordigers.Value,
            _entityId,
            _expectedVervaldag,
            BewaartermijnReden.VertegenwoordigerWerdVerwijderd
        );

        var messageHandler = new StartBewaartermijnCommandHandler();
        _eventStore = new Mock<IEventStore>();
        _notifier = new Mock<INotifier>();
        messageHandler
            .Handle(message, _eventStore.Object, _notifier.Object, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_The_Bewaartermijn_Is_Saved()
    {
        var expectedAggregateId =
            $"{BewaartermijnId.BewaartermijnAggregateName}-{_vCode}-{PersoonsgegevensType.Vertegenwoordigers.Value}-{_entityId}";

        _eventStore.Verify(x =>
            x.SaveNew(
                expectedAggregateId,
                It.Is<CommandMetadata>(metadata =>
                    metadata.Initiator == CommandMetadata.ForDigitaalVlaanderenProcess.Initiator
                    && metadata.ExpectedVersion == null
                ),
                It.IsAny<CancellationToken>(),
                It.Is<IEvent[]>(events =>
                    events.Length == 1
                    && ((BewaartermijnWerdGestartV2)events[0]).BewaartermijnId == expectedAggregateId
                    && ((BewaartermijnWerdGestartV2)events[0]).VCode == _vCode.ToString()
                    && ((BewaartermijnWerdGestartV2)events[0]).PersoonsgegevensType
                        == PersoonsgegevensType.Vertegenwoordigers.Value
                    && ((BewaartermijnWerdGestartV2)events[0]).EntityId == _entityId
                    && ((BewaartermijnWerdGestartV2)events[0]).Vervaldag == _expectedVervaldag
                    && ((BewaartermijnWerdGestartV2)events[0]).Reden
                        == BewaartermijnReden.VertegenwoordigerWerdVerwijderd
                )
            )
        );
    }
}
