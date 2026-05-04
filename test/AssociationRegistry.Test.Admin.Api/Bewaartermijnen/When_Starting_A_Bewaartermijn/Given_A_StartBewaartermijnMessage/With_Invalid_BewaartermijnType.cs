namespace AssociationRegistry.Test.Admin.Api.Bewaartermijnen.When_Starting_A_Bewaartermijn.Given_A_StartBewaartermijnMessage;

using AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Moq;
using NodaTime;
using Xunit;

public class With_Invalid_BewaartermijnType
{
    [Fact]
    public async Task Then_Throws_OngeldigBewaartermijnType()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var vCode = fixture.Create<VCode>();
        var entityId = fixture.Create<int>();
        var vervaldag = fixture.Create<Instant>();

        var invalidPersoonsgegevensType = fixture.Create<string>();

        var message = new StartBewaartermijnMessage(
            vCode,
            invalidPersoonsgegevensType,
            entityId,
            vervaldag,
            BewaartermijnReden.VertegenwoordigerWerdVerwijderd
        );

        var messageHandler = new StartBewaartermijnMessageHandler();
        var eventStore = new Mock<IEventStore>();

        var exception = await Assert.ThrowsAsync<OngeldigBewaartermijnType>(() =>
            messageHandler.Handle(message, eventStore.Object, CancellationToken.None)
        );

        exception.Message.Should().Be(ExceptionMessages.OngeldigBewaartermijnType);
    }
}
