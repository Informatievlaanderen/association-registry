namespace AssociationRegistry.Test.When_Heradresseren_Locaties;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using FluentAssertions;
using Grar;
using Grar.HeradresseerLocaties;
using Grar.Models;
using Moq;
using Xunit;

public class Given_Multiple_Message_With_Same_IdempotenceKey
{
    [Fact]
    public async Task Then_A_LocatieWerdToegevoegd_Event_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario);

        var fixture = new Fixture().CustomizeAdminApi();

        var mockedAdresDetail1 = fixture.Create<AddressDetailResponse>();
        var mockedAdresDetail2 = fixture.Create<AddressDetailResponse>();

        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddressById("123", CancellationToken.None))
                      .ReturnsAsync(mockedAdresDetail1);

        grarClientMock.Setup(x => x.GetAddressById("456", CancellationToken.None))
                      .ReturnsAsync(mockedAdresDetail2);

        var idempotenceKey = "123456789";
        var locatieId1 = scenario.Locaties.First().LocatieId;
        var locatieId2 = scenario.Locaties.ToArray()[1].LocatieId;

        var message1 = fixture.Create<TeHeradresserenLocatiesMessage>() with
        {
            LocatiesMetAdres = new List<LocatieIdWithAdresId>
                { new(locatieId1, AddressId: "123"), new(locatieId2, AddressId: "456") },
            VCode = scenario.VCode,
            idempotencyKey = idempotenceKey,
        };

        var message2 = fixture.Create<TeHeradresserenLocatiesMessage>() with
        {
            LocatiesMetAdres = new List<LocatieIdWithAdresId>
                { new(locatieId1, AddressId: "456"), new(locatieId2, AddressId: "123") },
            VCode = scenario.VCode,
            idempotencyKey = idempotenceKey + 1,
        };

        var messageHandler = new TeHeradresserenLocatiesMessageHandler(verenigingRepositoryMock, grarClientMock.Object);

        await messageHandler.Handle(message1, CancellationToken.None);
        await messageHandler.Handle(message2, CancellationToken.None);
        await messageHandler.Handle(message1, CancellationToken.None); // idempotent message

        verenigingRepositoryMock.SaveInvocations[0].Vereniging.UncommittedEvents.Should()
                                .BeEquivalentTo(
                                     new List<IEvent>
                                     {
                                         new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value,
                                                                                  locatieId1,
                                                                                  mockedAdresDetail1.AdresId,
                                                                                  mockedAdresDetail1.ToAdresUitAdressenregister(),
                                                                                  message1.idempotencyKey),
                                         new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId2,
                                                                                  mockedAdresDetail2.AdresId,
                                                                                  mockedAdresDetail2.ToAdresUitAdressenregister(),
                                                                                  message1.idempotencyKey),
                                     }
                                   , config: options => options.RespectingRuntimeTypes().WithStrictOrdering());

        verenigingRepositoryMock.SaveInvocations[1].Vereniging.UncommittedEvents.Should()
                                .BeEquivalentTo(
                                     new List<IEvent>
                                     {
                                         new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId1,
                                                                                  mockedAdresDetail2.AdresId,
                                                                                  mockedAdresDetail2.ToAdresUitAdressenregister(),
                                                                                  message2.idempotencyKey),
                                         new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId2,
                                                                                  mockedAdresDetail1.AdresId,
                                                                                  mockedAdresDetail1.ToAdresUitAdressenregister(),
                                                                                  message2.idempotencyKey),
                                     }
                                   , config: options => options.RespectingRuntimeTypes().WithStrictOrdering());

        verenigingRepositoryMock.SaveInvocations[2].Vereniging.UncommittedEvents.Should().BeEmpty();
    }
}
