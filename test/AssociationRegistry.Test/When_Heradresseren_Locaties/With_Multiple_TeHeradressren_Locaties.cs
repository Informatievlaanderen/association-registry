﻿namespace AssociationRegistry.Test.When_Heradresseren_Locaties;

using AssociationRegistry.Events;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.HeradresseerLocaties;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Moq;
using Xunit;

public class With_Multiple_TeHeradressren_Locaties
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

        var locatieId1 = scenario.Locaties.First().LocatieId;
        var locatieId2 = scenario.Locaties.ToArray()[1].LocatieId;

        var message = fixture.Create<TeHeradresserenLocatiesMessage>() with
        {
            LocatiesMetAdres = new List<LocatieIdWithAdresId>() { new(locatieId1, "123"), new(locatieId2, "456") },
            VCode = "V001",
            idempotencyKey = "123456789",
        };

        var messageHandler = new TeHeradresserenLocatiesMessageHandler(verenigingRepositoryMock, grarClientMock.Object);

        await messageHandler.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldHaveSaved(
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
