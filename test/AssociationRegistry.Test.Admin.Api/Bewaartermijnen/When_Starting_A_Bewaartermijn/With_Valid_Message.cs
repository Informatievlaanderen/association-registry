namespace AssociationRegistry.Test.Admin.Api.Bewaartermijnen.When_Starting_A_Bewaartermijn;

using AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using CommandHandling.Bewaartermijnen;
using Events;
using MartenDb.Store;
using Moq;
using Xunit;

public class With_Valid_Message
{
    private Mock<IEventStore> _eventStore;
    private readonly VCode _vCode;
    private readonly int _vertegenwoordigerId;
    private CommandMetadata? _commandMetadata;

    public With_Valid_Message()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        _vCode = fixture.Create<VCode>();
        _vertegenwoordigerId = fixture.Create<int>();
        var command = new StartBewaartermijnMessage(_vCode, _vertegenwoordigerId);
        _commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new StartBewaartermijnMessageHandler();
        _eventStore = new Mock<IEventStore>();
        commandHandler.Handle(new CommandEnvelope<StartBewaartermijnMessage>(command, _commandMetadata), _eventStore.Object, CancellationToken.None)
                      .GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Bewaartermijn_Is_Saved()
    {
        var expectedAggregateId = $"{_vCode}-{_vertegenwoordigerId}";

        _eventStore.Verify(x => x.Save(expectedAggregateId,
                                       0,
                                       _commandMetadata,
                                       It.IsAny<CancellationToken>(),
                                       new BewaartermijnWerdGestart(expectedAggregateId, _vCode.ToString(), _vertegenwoordigerId)));
    }

    // [Fact]
    // public void Then_It_Outboxes_An_StartBewaartermijn_Message()
    // {
    //     _outbox.Verify(x => x.SendAsync(new StartBewaartermijnMessage(), It.IsAny<DeliveryOptions>()), Times.Once);
    // }
    //
    // [Fact]
    // public void Then_A_VertegenwoordigerWerdVerwijderd_Event_Is_Saved()
    // {
    //     _verenigingRepositoryMock.ShouldHaveSavedExact(
    //         new VertegenwoordigerWerdVerwijderd(
    //             _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
    //             _scenario.VertegenwoordigerWerdToegevoegd.Insz,
    //             _scenario.VertegenwoordigerWerdToegevoegd.Voornaam,
    //             _scenario.VertegenwoordigerWerdToegevoegd.Achternaam)
    //     );
    // }
}
