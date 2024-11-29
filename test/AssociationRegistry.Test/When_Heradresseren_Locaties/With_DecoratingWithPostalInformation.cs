namespace AssociationRegistry.Test.When_Heradresseren_Locaties;

using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Grar;
using Grar.GrarUpdates.TeHeradresserenLocaties;
using Grar.Models;
using Grar.Models.PostalInfo;
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
                    Adres = Adres.Create(straatnaam: "straat", huisnummer: "14", busnummer: "", postcode: "1790",
                                         gemeente: "Hekelgem (Affligem)", land: "België"),
                },
            }),
        };

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario);

        var mockedAdresDetail = fixture.Create<AddressDetailResponse>();

        var mockedPostalInformation = new PostalInformationResponse(mockedAdresDetail.Postcode,
                                                                    Gemeentenaam: "Affligem",
                                                                    Postnamen.FromValues("Hekelgem"));

        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddressById("123", CancellationToken.None))
                      .ReturnsAsync(mockedAdresDetail);

        grarClientMock.Setup(x => x.GetPostalInformation(mockedAdresDetail.Postcode))
                      .ReturnsAsync(mockedPostalInformation);

        var locatieId = scenario.Locaties.First().LocatieId;

        var message = fixture.Create<TeHeradresserenLocatiesMessage>() with
        {
            TeHeradresserenLocaties = new List<TeHeradresserenLocatie>
                { new(locatieId, DestinationAdresId: "123") },
            VCode = "V001",
            idempotencyKey = "123456789",
        };

        var messageHandler = new TeHeradresserenLocatiesMessageHandler(verenigingRepositoryMock, grarClientMock.Object);

        var expectedAdres = new AdresDetailUitAdressenregister
        {
            Adres = new Registratiedata.AdresUitAdressenregister(mockedAdresDetail.Straatnaam,
                                                                 mockedAdresDetail.Huisnummer,
                                                                 mockedAdresDetail.Busnummer,
                                                                 mockedAdresDetail.Postcode,
                                                                 Gemeente: "Hekelgem (Affligem)"
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
