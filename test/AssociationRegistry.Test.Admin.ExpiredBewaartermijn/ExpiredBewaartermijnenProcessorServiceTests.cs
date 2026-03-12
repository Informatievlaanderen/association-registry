namespace AssociationRegistry.Test.Admin.ExpiredBewaartermijn;

using AssociationRegistry.Admin.ExpiredBewaartermijnProcessor;
using AssociationRegistry.Admin.ExpiredBewaartermijnProcessor.Queries;
using AssociationRegistry.Admin.Schema.Bewaartermijn;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.Bewaartermijnen.Acties.Verlopen;
using Common.AutoFixture;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NodaTime;
using Wolverine;

public class VerlopenBewaartermijnenProcessorTests
{

    [Fact]
    public async Task Given_No_Verlopen_Bewaartermijnen_Then_MessageBus_Is_Never_Called()
    {
        var query = new Mock<IVerlopenBewaartermijnQuery>();

        query
           .Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(Array.Empty<BewaartermijnDocument>());

        var messageBus = new Mock<IMessageBus>();
        messageBus
           .Setup(x => x.SendAsync(It.IsAny<CommandEnvelope<VerwijderVertegenwoordigerPersoonsgegevensCommand>>(), It.IsAny<DeliveryOptions?>()))
           .Returns(ValueTask.CompletedTask);

        var sut = new VerlopenBewaartermijnenProcessor(
            query.Object,
            messageBus.Object,
            NullLogger<VerlopenBewaartermijnenProcessor>.Instance);

        await sut.SendVerlopenBewaartermijnen(CancellationToken.None);

        messageBus.Verify(
            x => x.SendAsync(It.IsAny<CommandEnvelope<VerwijderVertegenwoordigerPersoonsgegevensCommand>>(),It.IsAny<DeliveryOptions?>()),
            Times.Never);
    }

    [Fact]
    public async Task Given_2_Verlopen_Bewaartermijnen_Then_MessageBus_Is_Invoked_Twice_With_Correct_Commands()
    {
        var expiredBewaartermijnen = await VerlopenBewaartermijnen();

        var query = new Mock<IVerlopenBewaartermijnQuery>();
        query
           .Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(expiredBewaartermijnen);

        var messageBus = new Mock<IMessageBus>();

        messageBus
           .Setup(x => x.SendAsync(It.IsAny<CommandEnvelope<VerwijderVertegenwoordigerPersoonsgegevensCommand>>(),
                                   It.IsAny<DeliveryOptions?>()))
           .Returns(ValueTask.CompletedTask);

        var sut = new VerlopenBewaartermijnenProcessor(
            query.Object,
            messageBus.Object,
            NullLogger<VerlopenBewaartermijnenProcessor>.Instance);

        await sut.SendVerlopenBewaartermijnen(CancellationToken.None);

        messageBus.Verify(
            x => x.SendAsync(It.IsAny<CommandEnvelope<VerwijderVertegenwoordigerPersoonsgegevensCommand>>(), It.IsAny<DeliveryOptions?>()),
            Times.Exactly(2));
    }

     private Task<BewaartermijnDocument[]> VerlopenBewaartermijnen()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        return Task.FromResult<BewaartermijnDocument[]>([
            fixture.Create<BewaartermijnDocument>() with
            {
                Status = BewaartermijnStatus.Gepland.StatusNaam,
                Vervaldag = VerlopenVervaldag(fixture),
            },
            fixture.Create<BewaartermijnDocument>() with
            {
                Status = BewaartermijnStatus.Gepland.StatusNaam,
                Vervaldag = VerlopenVervaldag(fixture),
            },
        ]);
    }

    private static Instant VerlopenVervaldag(Fixture fixture)
        => SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(fixture.Create<int>()));

}
