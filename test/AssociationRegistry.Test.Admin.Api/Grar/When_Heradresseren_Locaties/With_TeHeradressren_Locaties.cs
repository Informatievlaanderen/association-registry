namespace AssociationRegistry.Test.Admin.Api.Grar.When_Heradresseren_Locaties;

using Api.Fixtures.Scenarios.CommandHandling;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.HeradresseerLocaties;
using AssociationRegistry.Grar.Models;
using AutoFixture;
using Events;
using Fakes;
using FluentAssertions;
using Framework;
using Moq;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_TeHeradressren_Locaties
{
    [Fact]
    public async Task Then_A_LocatieWerdToegevoegd_Event_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario);

        var fixture = new Fixture().CustomizeAdminApi();

        var mockedAdresDetail = fixture.Create<AddressDetailResponse>();

        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddressById("123", CancellationToken.None))
                      .ReturnsAsync(mockedAdresDetail);

        var locatieId = scenario.Locaties.First().LocatieId;

        var message = fixture.Create<TeHeradresserenLocatiesMessage>() with
        {
            LocatiesMetAdres = new List<LocatieIdWithAdresId>() { new(locatieId, "123") },
            VCode = "V001",
            idempotencyKey = "123456789"
        };

        var messageHandler = new TeHeradresserenLocatiesMessageHandler(verenigingRepositoryMock, grarClientMock.Object);

        await messageHandler.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldHaveSaved(
            new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId,
                                                     AdresDetailUitAdressenregister.FromResponse(mockedAdresDetail), message.idempotencyKey)
        );
    }
}

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
            idempotencyKey = "123456789"
        };

        var messageHandler = new TeHeradresserenLocatiesMessageHandler(verenigingRepositoryMock, grarClientMock.Object);

        await messageHandler.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldHaveSaved(
            new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId1,
                                                     AdresDetailUitAdressenregister.FromResponse(mockedAdresDetail1),
                                                     message.idempotencyKey),
            new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId2,
                                                     AdresDetailUitAdressenregister.FromResponse(mockedAdresDetail2),
                                                     message.idempotencyKey)
        );
    }
}

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
            LocatiesMetAdres = new List<LocatieIdWithAdresId>() { new(locatieId1, "123"), new(locatieId2, "456") },
            VCode = scenario.VCode,
            idempotencyKey = idempotenceKey
        };

        var message2 = fixture.Create<TeHeradresserenLocatiesMessage>() with
        {
            LocatiesMetAdres = new List<LocatieIdWithAdresId>() { new(locatieId1, "456"), new(locatieId2, "123") },
            VCode = scenario.VCode,
            idempotencyKey = idempotenceKey + 1
        };

        var messageHandler = new TeHeradresserenLocatiesMessageHandler(verenigingRepositoryMock, grarClientMock.Object);

        await messageHandler.Handle(message1, CancellationToken.None);
        await messageHandler.Handle(message2, CancellationToken.None);
        await messageHandler.Handle(message1, CancellationToken.None); // idempotent message

        verenigingRepositoryMock.SaveInvocations[0].Vereniging.UncommittedEvents.Should()
                                .BeEquivalentTo(
                                     new List<IEvent>()
                                     {
                                         new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId1,
                                                                                  AdresDetailUitAdressenregister.FromResponse(
                                                                                      mockedAdresDetail1), message1.idempotencyKey),
                                         new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId2,
                                                                                  AdresDetailUitAdressenregister.FromResponse(
                                                                                      mockedAdresDetail2), message1.idempotencyKey)
                                     }
                                   , config: options => options.RespectingRuntimeTypes().WithStrictOrdering());

        verenigingRepositoryMock.SaveInvocations[1].Vereniging.UncommittedEvents.Should()
                                .BeEquivalentTo(
                                     new List<IEvent>()
                                     {
                                         new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId1,
                                                                                  AdresDetailUitAdressenregister.FromResponse(
                                                                                      mockedAdresDetail2), message2.idempotencyKey),
                                         new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId2,
                                                                                  AdresDetailUitAdressenregister.FromResponse(
                                                                                      mockedAdresDetail1), message2.idempotencyKey)
                                     }
                                   , config: options => options.RespectingRuntimeTypes().WithStrictOrdering());

        verenigingRepositoryMock.SaveInvocations[2].Vereniging.UncommittedEvents.Should().BeEmpty();
    }
}

public class With_DecoratingWithPostalInformation
{
    [Fact]
    public async Task Then_A_LocatieWerdToegevoegd_Event_Is_Saved()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState() with
        {
            Locaties = Locaties.Empty.Hydrate(new[]
            {
                fixture.Create<Locatie>() with
                {
                    Adres = Adres.Create("straat", "14", "", "1790", "Hekelgem (Affligem)", "Belgie")
                },
            }),
        };

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario);

        var mockedAdresDetail = fixture.Create<AddressDetailResponse>();

        var mockedPostalInformation = new PostalInformationResponse(mockedAdresDetail.Postcode,
                                                                    "Affligem",
                                                                    new[] { "Hekelgem" });

        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddressById("123", CancellationToken.None))
                      .ReturnsAsync(mockedAdresDetail);

        grarClientMock.Setup(x => x.GetPostalInformation(mockedAdresDetail.Postcode))
                      .ReturnsAsync(mockedPostalInformation);

        var locatieId = scenario.Locaties.First().LocatieId;

        var message = fixture.Create<TeHeradresserenLocatiesMessage>() with
        {
            LocatiesMetAdres = new List<LocatieIdWithAdresId>() { new(locatieId, "123") },
            VCode = "V001",
            idempotencyKey = "123456789",
        };

        var messageHandler = new TeHeradresserenLocatiesMessageHandler(verenigingRepositoryMock, grarClientMock.Object);

        var expectedAdres = new AdresDetailUitAdressenregister()
        {
            Adres = new Registratiedata.Adres(mockedAdresDetail.Straatnaam,
                                              mockedAdresDetail.Huisnummer,
                                              mockedAdresDetail.Busnummer,
                                              mockedAdresDetail.Postcode,
                                              "Hekelgem (Affligem)",
                                              "België"),
            AdresId = mockedAdresDetail.AdresId,
        };

        await messageHandler.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldHaveSaved(
            new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId,
                                                     expectedAdres, message.idempotencyKey)
        );
    }
}
