namespace AssociationRegistry.Test.Admin.Api.Bewaartermijnen.When_Starting_A_Bewaartermijn;

using AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using CommandHandling.Bewaartermijnen;
using Events;
using Integrations.Grar.Bewaartermijnen;
using MartenDb.Store;
using Moq;
using Xunit;

public class With_Valid_Message
{
    private Mock<IEventStore> _eventStore;
    private readonly VCode _vCode;
    private readonly int _vertegenwoordigerId;
    private CommandMetadata _commandMetadata;
    private BewaartermijnOptions _bewaartermijnOptions;

    public With_Valid_Message()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        _vCode = fixture.Create<VCode>();
        _vertegenwoordigerId = fixture.Create<int>();
        var command = new StartBewaartermijnMessage(_vCode, _vertegenwoordigerId);
        _commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new StartBewaartermijnMessageHandler();
        _eventStore = new Mock<IEventStore>();
        _bewaartermijnOptions = new BewaartermijnOptions(){
            Duration = TimeSpan.FromDays(1),
        };

        commandHandler.Handle(new CommandEnvelope<StartBewaartermijnMessage>(command, _commandMetadata), _eventStore.Object, _bewaartermijnOptions, CancellationToken.None)
                      .GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Bewaartermijn_Is_Saved()
    {
        var expectedAggregateId = $"{_vCode}-{_vertegenwoordigerId}";
        var expectedVervaldag = _commandMetadata.Tijdstip.PlusTicks(_bewaartermijnOptions.Duration.Ticks);

        _eventStore.Verify(x => x.Save(expectedAggregateId,
                                       0,
                                       _commandMetadata,
                                       It.IsAny<CancellationToken>(),
                                       new BewaartermijnWerdGestart(expectedAggregateId, _vCode.ToString(), _vertegenwoordigerId, expectedVervaldag)));
    }

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
