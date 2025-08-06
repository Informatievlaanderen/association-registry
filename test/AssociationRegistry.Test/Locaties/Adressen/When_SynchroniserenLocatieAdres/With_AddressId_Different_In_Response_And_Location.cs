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
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Geotags;
using Events;
using Events.Factories;
using JasperFx.Events;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging;
using Xunit;
using IEvent = Events.IEvent;

public class With_AddressId_Different_In_Response_And_Location
{
    private VerenigingRepositoryMock _verenigingRepositoryMock;
    private Mock<IGrarClient> _grarClientMock;
    private Mock<IGeotagsService> _geotagsService;
    private GeotagsCollection _geotags;
    private Fixture _fixture;
    private VerenigingState _state;

    public With_AddressId_Different_In_Response_And_Location()
    {
        _fixture = new Fixture().CustomizeDomain();
        _state = new FeitelijkeVerenigingWerdGeregistreerdScenario().GetVerenigingState();

        var factory = new Faktory(_fixture);
        _verenigingRepositoryMock = factory.VerenigingsRepository.Mock(_state, expectedLoadingDubbel: true);
        (_geotagsService, _geotags) = factory.GeotagsService.ReturnsRandomGeotags();
        _grarClientMock = new Mock<IGrarClient>();

    }

    [Fact]
    public async ValueTask Then_ItClearsBothAddressAndAddressId()
    {
        var locatie = _state.Locaties.First();

        var command = _fixture.Create<SyncAdresLocatiesCommand>() with
        {
            LocatiesWithAdres = new List<LocatieWithAdres>
                { new(locatie.LocatieId, Adres: null) },
            VCode = "V001",
            IdempotenceKey = "123456789",
        };

        var commandHandler = new SyncAdresLocatiesCommandHandler(_verenigingRepositoryMock, _grarClientMock.Object,
                                                                 new NullLogger<SyncAdresLocatiesCommandHandler>(),
                                                                 _geotagsService.Object);

        await commandHandler.Handle(command, CancellationToken.None);

        IEvent[] addedEvents = [new AdresWerdOntkoppeldVanAdressenregister(
                _state.VCode.Value,
                locatie.LocatieId,
                EventFactory.AdresId(locatie.AdresId),
                EventFactory.Adres(locatie.Adres)),
            EventFactory.GeotagsWerdenBepaald(_state.VCode, _geotags)];

        _verenigingRepositoryMock.ShouldHaveSavedExact(addedEvents);

        var stateWithAddedEvents = addedEvents.Aggregate(_state, (state, @event) => state.Apply((dynamic)@event));
        var repositoryMockWithAddedEvents = Faktory.New().VerenigingsRepository.Mock(stateWithAddedEvents);
        var commandHandlerWithAddedEvents = new SyncAdresLocatiesCommandHandler(repositoryMockWithAddedEvents, _grarClientMock.Object,
                                                                                new NullLogger<SyncAdresLocatiesCommandHandler>(),
                                                                                _geotagsService.Object);
        await commandHandlerWithAddedEvents.Handle(command, CancellationToken.None);

        repositoryMockWithAddedEvents.ShouldNotHaveAnySaves();
    }
}
