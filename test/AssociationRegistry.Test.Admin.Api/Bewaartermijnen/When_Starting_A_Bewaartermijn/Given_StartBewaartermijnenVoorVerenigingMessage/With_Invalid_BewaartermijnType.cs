namespace AssociationRegistry.Test.Admin.Api.Bewaartermijnen.When_Starting_A_Bewaartermijn.Given_StartBewaartermijnenVoorVerenigingMessage;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.Bewaartermijnen.Acties.Start;
using CommandHandling.Bewaartermijnen.Acties.Start.Notifications;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Integrations.Grar.Bewaartermijnen;
using Integrations.Slack;
using Moq;
using Resources;
using Xunit;

public class With_Invalid_BewaartermijnType
{
    private readonly StartBewaartermijnenVoorVerenigingMessage _message;
    private readonly StartBewaartermijnenVoorVerenigingMessageHandler _commandHandler;
    private readonly Mock<IEventStore> _eventStore;
    private readonly Mock<INotifier> _notifier;

    public With_Invalid_BewaartermijnType()
    {
        var fixture = new Fixture();

        var commandMetadata = CommandMetadata.ForDigitaalVlaanderenProcess;
        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
        var bewaartermijnOptions = new BewaartermijnOptions() { Duration = TimeSpan.FromDays(1) };
        var expectedVervaldag = commandMetadata.Tijdstip.PlusTicks(bewaartermijnOptions.Duration.Ticks);

        var invalidPersoonsgegevensType = fixture.Create<string>();

        _message = new StartBewaartermijnenVoorVerenigingMessage(
            scenario.VCode,
            invalidPersoonsgegevensType,
            expectedVervaldag,
            BewaartermijnReden.VerenigingWerdVerwijderd
        );

        _commandHandler = new StartBewaartermijnenVoorVerenigingMessageHandler();
        _eventStore = new Mock<IEventStore>();
        _notifier = new Mock<INotifier>();

        _eventStore.Setup(x => x.Load<VerenigingState>(scenario.VCode, null)).ReturnsAsync(scenario.GetVerenigingState);
    }

    [Fact]
    public async Task Then_Throws_OngeldigBewaartermijnType()
    {
        var exception = await Assert.ThrowsAsync<OngeldigBewaartermijnType>(() =>
            _commandHandler.Handle(_message, _eventStore.Object, _notifier.Object, CancellationToken.None)
        );

        exception.Message.Should().Be(ExceptionMessages.OngeldigBewaartermijnType);
    }

    [Fact]
    public async Task Then_A_Slack_Notification_Is_Sent()
    {
        await Assert.ThrowsAsync<OngeldigBewaartermijnType>(() =>
            _commandHandler.Handle(_message, _eventStore.Object, _notifier.Object, CancellationToken.None)
        );

        _notifier.Verify(
            x =>
                x.Notify(
                    It.Is<INotification>(n => n is EventSubscriptionBewaartermijnFailed && n.Type == NotifyType.Failure)
                ),
            Times.Once
        );
    }
}
