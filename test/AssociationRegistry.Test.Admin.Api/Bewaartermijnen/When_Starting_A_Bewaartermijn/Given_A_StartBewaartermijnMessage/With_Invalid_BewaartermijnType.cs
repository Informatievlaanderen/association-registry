namespace AssociationRegistry.Test.Admin.Api.Bewaartermijnen.When_Starting_A_Bewaartermijn.Given_A_StartBewaartermijnCommand;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using AutoFixture;
using CommandHandling.Bewaartermijnen.Acties.Start;
using CommandHandling.Bewaartermijnen.Acties.Start.Notifications;
using Common.AutoFixture;
using FluentAssertions;
using Integrations.Slack;
using Moq;
using NodaTime;
using Resources;
using Xunit;

public class With_Invalid_BewaartermijnType
{
    private readonly StartBewaartermijnCommand _message;
    private readonly StartBewaartermijnCommandHandler _messageHandler;
    private readonly Mock<IEventStore> _eventStore;
    private readonly Mock<INotifier> _notifier;

    public With_Invalid_BewaartermijnType()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var vCode = fixture.Create<VCode>();
        var entityId = fixture.Create<int>();
        var vervaldag = fixture.Create<Instant>();

        var invalidPersoonsgegevensType = fixture.Create<string>();

        _message = new StartBewaartermijnCommand(
            vCode,
            invalidPersoonsgegevensType,
            entityId,
            vervaldag,
            BewaartermijnReden.VertegenwoordigerWerdVerwijderd
        );

        _messageHandler = new StartBewaartermijnCommandHandler();
        _eventStore = new Mock<IEventStore>();
        _notifier = new Mock<INotifier>();
    }

    [Fact]
    public async Task Then_Throws_OngeldigBewaartermijnType()
    {
        var exception = await Assert.ThrowsAsync<OngeldigBewaartermijnType>(() =>
            _messageHandler.Handle(_message, _eventStore.Object, _notifier.Object, CancellationToken.None)
        );

        exception.Message.Should().Be(ExceptionMessages.OngeldigBewaartermijnType);
    }

    [Fact]
    public async Task Then_A_Slack_Notification_Is_Sent()
    {
        await Assert.ThrowsAsync<OngeldigBewaartermijnType>(() =>
            _messageHandler.Handle(_message, _eventStore.Object, _notifier.Object, CancellationToken.None)
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
