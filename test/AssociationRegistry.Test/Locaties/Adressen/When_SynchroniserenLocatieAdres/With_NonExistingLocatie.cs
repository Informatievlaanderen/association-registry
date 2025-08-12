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
using Events.Factories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class With_NonExistingLocatie
{
    [Fact]
    public async ValueTask Then_An_AdresWerdOntkoppeldVanAdressenregister_Was_Saved()
    {
        var state = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var fixture = new Fixture().CustomizeDomain();

        var mockedAdresDetail = fixture.Create<AddressDetailResponse>() with
        {
            IsActief = false,
        };

        var factory = new Faktory(fixture);
        var verenigingRepositoryMock = factory.VerenigingsRepository.Mock(state, expectedLoadingDubbel: true);
        (var geotagsService, var geotags) = factory.GeotagsService.ReturnsRandomGeotags();

        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddressById("123", CancellationToken.None))
                      .ReturnsAsync(mockedAdresDetail);

        var locatieId = state.Locaties.First().LocatieId;

        var nonExistingLocatieId = locatieId * -1;

        var command = fixture.Create<SyncAdresLocatiesCommand>() with
        {
            LocatiesWithAdres = new List<LocatieWithAdres>
                { new(nonExistingLocatieId, mockedAdresDetail) },
            VCode = "V001",
            IdempotenceKey = "123456789",
        };

        var commandHandler = new SyncAdresLocatiesCommandHandler(verenigingRepositoryMock, grarClientMock.Object,
                                                                 new NullLogger<SyncAdresLocatiesCommandHandler>(),
                                                                 geotagsService.Object);

        await commandHandler.Handle(command, CancellationToken.None);

        verenigingRepositoryMock.ShouldHaveSavedExact(
            EventFactory.GeotagsWerdenBepaald(state.VCode, geotags));
    }
}
