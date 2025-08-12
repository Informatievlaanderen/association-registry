namespace AssociationRegistry.Test.Locaties.When_Heradresseren_Locaties;

using AssociationRegistry.Events;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AutoFixture;
using CommandHandling.Grar.GrarConsumer.Messaging.HeradresseerLocaties;
using CommandHandling.Grar.GrarUpdates.Hernummering;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Common.StubsMocksFakes.VerenigingsRepositories;
using AssociationRegistry.Integrations.Grar.Clients;
using Moq;
using Xunit;

public class With_Multiple_TeHeradressren_Locaties
{
    [Fact]
    public async ValueTask Then_A_LocatieWerdToegevoegd_Event_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario, expectedLoadingDubbel: true);

        var fixture = new Fixture().CustomizeAdminApi();

        var mockedAdresDetail1 = fixture.Create<AddressDetailResponse>();
        var mockedAdresDetail2 = fixture.Create<AddressDetailResponse>();

        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddressById("123", CancellationToken.None))
                      .ReturnsAsync(mockedAdresDetail1);

        grarClientMock.Setup(x => x.GetAddressById("456", CancellationToken.None))
                      .ReturnsAsync(mockedAdresDetail2);

        var locatieId1 = scenario.Locaties.First().LocatieId;
        var locatieId2 = scenario.Locaties.ToArray()[1].LocatieId;

        var message = fixture.Create<HeradresseerLocatiesMessage>() with
        {
            TeHeradresserenLocaties = new List<TeHeradresserenLocatie>
                { new(locatieId1, NaarAdresId: "123"), new(locatieId2, NaarAdresId: "456") },
            VCode = "V001",
            idempotencyKey = "123456789",
        };

        var messageHandler = new HeradresseerLocatiesMessageHandler(verenigingRepositoryMock, grarClientMock.Object);

        await messageHandler.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldHaveSavedExact(
            new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId1,
                                                     mockedAdresDetail1.AdresId,
                                                     mockedAdresDetail1.ToAdresUitAdressenregister(),
                                                     message.idempotencyKey),
            new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId2,
                                                     mockedAdresDetail2.AdresId,
                                                     mockedAdresDetail2.ToAdresUitAdressenregister(),
                                                     message.idempotencyKey)
        );
    }
}
