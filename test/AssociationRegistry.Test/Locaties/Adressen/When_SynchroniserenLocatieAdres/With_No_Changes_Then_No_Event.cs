namespace AssociationRegistry.Test.Locaties.Adressen.When_SynchroniserenLocatieAdres;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models;
using AutoFixture;
using CommandHandling.Grar.NightlyAdresSync.SyncAdresLocaties;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Events.Factories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class With_No_Changes_Then_No_Event
{
    [Fact]
    public async ValueTask Then_A_LocatieWerdToegevoegd_Event_Is_Saved()
    {
        var state = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var fixture = new Fixture().CustomizeDomain();
        var locatie = state.Locaties.First();

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
                AdresId = new Registratiedata.AdresId(locatie.AdresId.Adresbron.Code, locatie.AdresId.Bronwaarde),
                Straatnaam = locatie.Adres.Straatnaam,
                Busnummer = locatie.Adres.Busnummer,
                Gemeente = locatie.Adres.Gemeente.Naam,
                Huisnummer = locatie.Adres.Huisnummer,
                Adresvoorstelling = locatie.Adres.ToAdresString(),
                Postcode = locatie.Adres.Postcode,
                IsActief = true,
            };

        var factory = new Faktory(fixture);
        var verenigingRepositoryMock = factory.VerenigingsRepository.Mock(state, expectedLoadingDubbel: true);
        (var geotagsService, var geotags) = factory.GeotagsService.ReturnsRandomGeotags();

        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddressById("123", CancellationToken.None))
                      .ReturnsAsync(mockedAdresDetail);

        var command = fixture.Create<SyncAdresLocatiesCommand>() with
        {
            LocatiesWithAdres = new List<LocatieWithAdres>
                { new(locatie.LocatieId, mockedAdresDetail) },
            VCode = "V001",
            IdempotenceKey = "123456789",
        };

        var commandHandler = new SyncAdresLocatiesCommandHandler(verenigingRepositoryMock, grarClientMock.Object,
                                                                 new NullLogger<SyncAdresLocatiesCommandHandler>(),
                                                                 geotagsService.Object);

        await commandHandler.Handle(command, CancellationToken.None);

        verenigingRepositoryMock.ShouldHaveSavedExact(
            EventFactory.GeotagsWerdenBepaald(state.VCode, geotags));
        verenigingRepositoryMock.ShouldNotHaveSaved<AdresWerdGewijzigdInAdressenregister>();
    }
}
