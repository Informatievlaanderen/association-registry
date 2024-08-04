namespace AssociationRegistry.Test.Admin.Api.Grar.When_Heradresseren_Locaties;

using Api.Fixtures.Scenarios.CommandHandling;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.HeradresseerLocaties;
using AssociationRegistry.Grar.Models;
using AutoFixture;
using Events;
using Fakes;
using Framework;
using Moq;
using Vereniging;
using Xunit;

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
                    Adres = Adres.Create("straat", "14", "", "1790", "Hekelgem (Affligem)", "België"),
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
            Adres = new Registratiedata.AdresUitAdressenregister(mockedAdresDetail.Straatnaam,
                                                                 mockedAdresDetail.Huisnummer,
                                                                 mockedAdresDetail.Busnummer,
                                                                 mockedAdresDetail.Postcode,
                                                                 "Hekelgem (Affligem)"
            ),
            AdresId = mockedAdresDetail.AdresId,
        };

        await messageHandler.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldHaveSaved(
            new AdresWerdGewijzigdInAdressenregister(scenario.VCode.Value, locatieId, expectedAdres.AdresId, expectedAdres.Adres,
                                                     message.idempotencyKey)
        );
    }
}
