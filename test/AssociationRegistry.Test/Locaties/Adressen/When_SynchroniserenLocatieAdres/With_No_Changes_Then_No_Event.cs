namespace AssociationRegistry.Test.Locaties.Adressen.When_SynchroniserenLocatieAdres;

using AssociationRegistry.Events;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.AddressSync;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AutoFixture;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_No_Changes_Then_No_Event
{
    [Fact]
    public async Task Then_A_LocatieWerdToegevoegd_Event_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario);

        var fixture = new Fixture().CustomizeDomain();
        var locatie = scenario.Locaties.First();

        locatie = locatie with
        {
            Adres = locatie.Adres with
            {
                Land = "België",
            },
        };

        var mockedAdresDetail = fixture.Create<AddressDetailResponse>()
            with
            {
                AdresId = new Registratiedata.AdresId(locatie.AdresId.Adresbron, locatie.AdresId.Bronwaarde),
                Straatnaam = locatie.Adres.Straatnaam,
                Busnummer = locatie.Adres.Busnummer,
                Gemeente = locatie.Adres.Gemeente.Naam,
                Huisnummer = locatie.Adres.Huisnummer,
                Adresvoorstelling = locatie.Adres.ToAdresString(),
                Postcode = locatie.Adres.Postcode,
                IsActief = true,
            };

        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddressById("123", CancellationToken.None))
                      .ReturnsAsync(mockedAdresDetail);

        var message = fixture.Create<TeSynchroniserenLocatieAdresMessage>() with
        {
            LocatiesWithAdres = new List<LocatieWithAdres>
                { new(locatie.LocatieId, mockedAdresDetail) },
            VCode = "V001",
            IdempotenceKey = "123456789",
        };

        var messageHandler = new TeSynchroniserenLocatieAdresMessageHandler(verenigingRepositoryMock, grarClientMock.Object,
                                                                            new NullLogger<TeSynchroniserenLocatieAdresMessageHandler>());

        await messageHandler.Handle(message, CancellationToken.None);

        verenigingRepositoryMock.ShouldNotHaveAnySaves();
        verenigingRepositoryMock.ShouldNotHaveSaved<AdresWerdGewijzigdInAdressenregister>();
    }
}
