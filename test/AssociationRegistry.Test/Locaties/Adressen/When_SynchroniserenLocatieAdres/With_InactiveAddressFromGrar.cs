namespace AssociationRegistry.Test.Locaties.Adressen.When_SynchroniserenLocatieAdres;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Grar.NightlyAdresSync.SyncAdresLocaties;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using EventFactories;
using Events;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class With_InactiveAddressFromGrar
{
    [Fact]
    public async ValueTask Then_An_AdresWerdOntkoppeldVanAdressenregister_Was_Saved()
    {
        var fixture = new Fixture().CustomizeDomain();
        var state = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var locatie = state.Locaties.First();
        var inactiefAdres = fixture.Create<AddressDetailResponse>() with
        {
            IsActief = false,
        };

        var command = fixture.Create<SyncAdresLocatiesCommand>() with
        {
            LocatiesWithAdres = new List<LocatieWithAdres>
                { new(locatie.LocatieId, inactiefAdres) },
            VCode = "V001",
            IdempotenceKey = "123456789",
        };

        var factory = new Faktory(fixture);
        var verenigingRepositoryMock = factory.VerenigingsRepository.Mock(state, expectedLoadingDubbel: true);
        (var geotagsService, var geotags) = factory.GeotagsService.ReturnsRandomGeotags();
        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddressById("123", CancellationToken.None))
                      .ReturnsAsync(inactiefAdres);

        var commandHandler = new SyncAdresLocatiesCommandHandler(verenigingRepositoryMock, grarClientMock.Object,
                                                                 new NullLogger<SyncAdresLocatiesCommandHandler>(),
                                                                 geotagsService.Object);

        await commandHandler.Handle(command, CancellationToken.None);

        verenigingRepositoryMock.ShouldHaveSavedExact(
            new AdresWerdOntkoppeldVanAdressenregister(state.VCode.Value,
                                                       locatie.LocatieId,
                                                       EventFactory.AdresId(locatie.AdresId),
                                                       EventFactory.Adres(locatie.Adres)),
            EventFactory.GeotagsWerdenBepaald(state.VCode, geotags));
    }
}
