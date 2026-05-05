namespace AssociationRegistry.Test.Admin.Api.Bewaartermijnen.When_Starting_A_Bewaartermijn.Given_StartBewaartermijnenVoorVerenigingMessage;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.Bewaartermijnen.Acties.Start;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Integrations.Grar.Bewaartermijnen;
using Integrations.Slack;
using Moq;
using NodaTime;
using Xunit;

public class With_Valid_Message
{
    private readonly Mock<IEventStore> _eventStore;
    private readonly CommandMetadata _commandMetadata;
    private readonly BewaartermijnOptions _bewaartermijnOptions;
    private readonly Instant _expectedVervaldag;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly Mock<INotifier> _notifier;

    public With_Valid_Message()
    {
        _commandMetadata = CommandMetadata.ForDigitaalVlaanderenProcess;
        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
        _bewaartermijnOptions = new BewaartermijnOptions() { Duration = TimeSpan.FromDays(1) };

        _expectedVervaldag = _commandMetadata.Tijdstip.PlusTicks(_bewaartermijnOptions.Duration.Ticks);

        var message = new StartBewaartermijnenVoorVerenigingMessage(
            _scenario.VCode,
            PersoonsgegevensType.Vertegenwoordigers.Value,
            _expectedVervaldag,
            BewaartermijnReden.VerenigingWerdVerwijderd
        );

        var commandHandler = new StartBewaartermijnenVoorVerenigingMessageHandler();
        _eventStore = new Mock<IEventStore>();
        _notifier = new Mock<INotifier>();

        _eventStore
            .Setup(x => x.Load<VerenigingState>(_scenario.VCode, null))
            .ReturnsAsync(_scenario.GetVerenigingState);

        commandHandler
            .Handle(message, _eventStore.Object, _notifier.Object, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_Foreach_Vertegenwoordiger_A_Bewaartermijn_Is_Saved()
    {
        foreach (
            var vertegenwoordiger in _scenario
                .VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                .Vertegenwoordigers
        )
        {
            var expectedAggregateId =
                $"{BewaartermijnId.BewaartermijnAggregateName}-{_scenario.VCode}-{PersoonsgegevensType.Vertegenwoordigers.Value}-{vertegenwoordiger.VertegenwoordigerId}";

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
                        && ((BewaartermijnWerdGestartV2)events[0]).VCode == _scenario.VCode.ToString()
                        && ((BewaartermijnWerdGestartV2)events[0]).PersoonsgegevensType
                            == PersoonsgegevensType.Vertegenwoordigers.Value
                        && ((BewaartermijnWerdGestartV2)events[0]).EntityId == vertegenwoordiger.VertegenwoordigerId
                        && ((BewaartermijnWerdGestartV2)events[0]).Vervaldag == _expectedVervaldag
                        && ((BewaartermijnWerdGestartV2)events[0]).Reden == BewaartermijnReden.VerenigingWerdVerwijderd
                    )
                )
            );
        }
    }
}
