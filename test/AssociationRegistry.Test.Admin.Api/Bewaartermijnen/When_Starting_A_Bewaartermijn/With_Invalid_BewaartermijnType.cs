namespace AssociationRegistry.Test.Admin.Api.Bewaartermijnen.When_Starting_A_Bewaartermijn;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.Bewaartermijnen.Acties.Start;
using Common.AutoFixture;
using FluentAssertions;
using Integrations.Grar.Bewaartermijnen;
using MartenDb.Store;
using Moq;
using Resources;
using Xunit;

public class With_Invalid_BewaartermijnType
{
    public With_Invalid_BewaartermijnType() { }

    [Fact]
    public async Task Then_Throws_OngeldigBewaartermijnType()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var vCode = fixture.Create<VCode>();
        var recordId = fixture.Create<int>();
        var commandMetadata = fixture.Create<CommandMetadata>();

        var bewaartermijnOptions = new BewaartermijnOptions() { Duration = TimeSpan.FromDays(1) };

        var command = new StartBewaartermijnMessage(
            vCode,
            fixture.Create<string>(),
            recordId,
            BewaartermijnReden.VertegenwoordigerWerdVerwijderd
        );

        var commandHandler = new StartBewaartermijnMessageHandler();
        var eventStore = new Mock<IEventStore>();

        var exception = await Assert.ThrowsAsync<OngeldigBewaartermijnType>(() =>
            commandHandler.Handle(
                new CommandEnvelope<StartBewaartermijnMessage>(command, commandMetadata),
                eventStore.Object,
                bewaartermijnOptions,
                CancellationToken.None
            )
        );

        exception.Message.Should().Be(ExceptionMessages.OngeldigBewaartermijnType);
    }
}
