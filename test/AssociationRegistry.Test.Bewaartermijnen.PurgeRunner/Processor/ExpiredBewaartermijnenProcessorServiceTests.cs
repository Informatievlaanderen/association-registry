namespace AssociationRegistry.Test.Bewaartermijnen.PurgeRunner.Processor;

using AssociationRegistry.Admin.Schema.Bewaartermijn;
using AssociationRegistry.Bewaartermijnen.PurgeRunner;
using AssociationRegistry.Bewaartermijnen.PurgeRunner.Queries;
using AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Verlopen;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
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

        var sut = new VerlopenBewaartermijnenProcessor(
            query.Object,
            messageBus.Object,
            NullLogger<VerlopenBewaartermijnenProcessor>.Instance
        );

        await sut.SendVerlopenBewaartermijnen(CancellationToken.None);

        messageBus.Verify(
            x =>
                x.InvokeAsync(
                    It.IsAny<CommandEnvelope<VerloopBewaartermijnCommand>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TimeSpan?>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task Given_2_Verlopen_Bewaartermijnen_Then_MessageBus_Is_Invoked_Twice_With_Correct_Commands()
    {
        var expiredBewaartermijnen = VerlopenBewaartermijnen();

        var query = new Mock<IVerlopenBewaartermijnQuery>();
        query.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expiredBewaartermijnen);

        var messageBus = new Mock<IMessageBus>();

        var sut = new VerlopenBewaartermijnenProcessor(
            query.Object,
            messageBus.Object,
            NullLogger<VerlopenBewaartermijnenProcessor>.Instance
        );

        await sut.SendVerlopenBewaartermijnen(CancellationToken.None);

        messageBus.Verify(
            x =>
                x.InvokeAsync(
                    It.IsAny<CommandEnvelope<VerloopBewaartermijnCommand>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TimeSpan?>()
                ),
            Times.Exactly(2)
        );
    }

    private BewaartermijnDocument[] VerlopenBewaartermijnen()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        return
        [
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
        ];
    }

    private static Instant VerlopenVervaldag(Fixture fixture) =>
        SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(fixture.Create<int>()));
}
