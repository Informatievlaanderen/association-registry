namespace AssociationRegistry.Test.Locaties.Adressen.When_SynchroniserenLocatieAdres;

using AssociationRegistry.Grar;
using AssociationRegistry.Integrations.Grar.Clients;
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
using Vereniging;
using Xunit;
using AddressDetailResponse = AssociationRegistry.Grar.Models.AddressDetailResponse;
using LocatieWithAdres = AssociationRegistry.Grar.Models.LocatieWithAdres;

public class With_A_Changed_Adres
{
    [Fact]
    public async ValueTask Then_A_AdresWerdGewijzigdInHetAdressenregiser()
    {
        var state = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();
        var locatieId = state.Locaties.First().LocatieId;

        var fixture = new Fixture().CustomizeDomain();

        var mockedAdresDetail = fixture.Create<AddressDetailResponse>() with { IsActief = true };

        var command = fixture.Create<SyncAdresLocatiesCommand>() with
        {
            LocatiesWithAdres = new List<LocatieWithAdres> { new(locatieId, mockedAdresDetail) },
            VCode = "V001",
            IdempotenceKey = "123456789",
        };
        var factory = new Faktory(fixture);
        var verenigingRepositoryMock = factory.AggregateSession.Mock(state, expectedLoadingDubbel: true);
        (var geotagsService, var geotags) = factory.GeotagsService.ReturnsRandomGeotags();
        var grarClientMock = new Mock<IGrarClient>();

        grarClientMock.Setup(x => x.GetAddressById("123", CancellationToken.None)).ReturnsAsync(mockedAdresDetail);

        var commandHandler = new SyncAdresLocatiesCommandHandler(
            verenigingRepositoryMock,
            grarClientMock.Object,
            new NullLogger<SyncAdresLocatiesCommandHandler>(),
            geotagsService.Object
        );

        await commandHandler.Handle(command, CancellationToken.None);

        verenigingRepositoryMock.ShouldHaveSavedExact(
            new AdresWerdGewijzigdInAdressenregister(
                state.VCode.Value,
                locatieId,
                mockedAdresDetail.AdresId,
                mockedAdresDetail.ToAdresUitAdressenregister(),
                command.IdempotenceKey
            ),
            EventFactory.GeotagsWerdenBepaald(state.VCode, geotags)
        );
    }
}
