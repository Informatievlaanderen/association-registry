namespace AssociationRegistry.Test.Admin.Api.Bewaartermijnen.When_Starting_A_Bewaartermijn;

using AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using CommandHandling.Bewaartermijnen;
using Events;
using Integrations.Grar.Bewaartermijnen;
using MartenDb.Store;
using Moq;
using NodaTime;
using Xunit;

public class With_Valid_Message
{
    private Mock<IEventStore> _eventStore;
    private readonly VCode _vCode;
    private readonly int _recordId;
    private CommandMetadata _commandMetadata;
    private BewaartermijnOptions _bewaartermijnOptions;
    private Instant _expectedVervaldag;

    public With_Valid_Message()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        _vCode = fixture.Create<VCode>();
        _recordId = fixture.Create<int>();
        _commandMetadata = fixture.Create<CommandMetadata>();

        _bewaartermijnOptions = new BewaartermijnOptions() { Duration = TimeSpan.FromDays(1) };

        _expectedVervaldag = _commandMetadata.Tijdstip.PlusTicks(_bewaartermijnOptions.Duration.Ticks);

        var command = new StartBewaartermijnMessage(
            _vCode,
            PersoonsgegevensType.Vertegenwoordigers.Value,
            _recordId,
            BewaartermijnReden.VertegenwoordigerWerdVerwijderd
        );

        var commandHandler = new StartBewaartermijnMessageHandler();
        _eventStore = new Mock<IEventStore>();

        commandHandler
            .Handle(
                new CommandEnvelope<StartBewaartermijnMessage>(command, _commandMetadata),
                _eventStore.Object,
                _bewaartermijnOptions,
                CancellationToken.None
            )
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_The_Bewaartermijn_Is_Saved()
    {
        var expectedAggregateId =
            $"{BewaartermijnId.BewaartermijnAggregateName}-{_vCode}-{PersoonsgegevensType.Vertegenwoordigers.Value}-{_recordId}";

        _eventStore.Verify(x =>
            x.SaveNew(
                expectedAggregateId,
                _commandMetadata,
                It.IsAny<CancellationToken>(),
                It.Is<IEvent[]>(events =>
                    events.Length == 1
                    && ((BewaartermijnWerdGestartV2)events[0]).BewaartermijnId == expectedAggregateId
                    && ((BewaartermijnWerdGestartV2)events[0]).VCode == _vCode.ToString()
                    && ((BewaartermijnWerdGestartV2)events[0]).PersoonsgegevensType
                        == PersoonsgegevensType.Vertegenwoordigers.Value
                    && ((BewaartermijnWerdGestartV2)events[0]).EntityId == _recordId
                    && ((BewaartermijnWerdGestartV2)events[0]).Vervaldag == _expectedVervaldag
                    && ((BewaartermijnWerdGestartV2)events[0]).Reden
                        == BewaartermijnReden.VertegenwoordigerWerdVerwijderd
                )
            )
        );
    }
}
