namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.
    CommandHandling;

using AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.EventFactories;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Messages;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging.Geotags;
using Wolverine;
using Wolverine.Marten;
using Xunit;

public class With_Locatie_With_AdresId
{
    private VerenigingRepositoryMock _verenigingRepositoryMock;
    private InMemorySequentialVCodeService _vCodeService;
    private ClockStub _clock;
    private Mock<IGrarClient> _grarClient;
    private Mock<IMartenOutbox> _martenOutbox;

    public With_Locatie_With_AdresId()
    {
        var faktory = Faktory.New();
        var fixture = new Fixture().CustomizeAdminApi();
        _verenigingRepositoryMock = faktory.VerenigingsRepository.Mock();
        _vCodeService = new InMemorySequentialVCodeService();
        _clock = faktory.Clock.Stub(fixture.Create<DateOnly>());
        _grarClient = new Mock<IGrarClient>();
        _martenOutbox = new Mock<IMartenOutbox>();

        const string naam = "De sjiekste club";

        var registreerCommand = fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();

        _grarClient.Setup(s => s.GetAddressById(locatie.AdresId.ToString(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(adresDetailResponses);


    }

    [Fact]
    public void Then_it_saves_the_event()
    {










        var geotag = new Geotag("BE32");
        var geotags = new[]
        {
            geotag
        };

        var geotagsService = new Mock<IGeotagsService>();
        geotagsService.Setup(x => x.CalculateGeotags(
                                 new[] {
                                     Locatie.Create(Locatienaam.Create(locatie.Naam),
                                                    locatie.IsPrimair,
                                                    locatie.Locatietype,
                                                    AdresId.Create(locatie.AdresId.Adresbron.Code, locatie.AdresId.Bronwaarde),
                                                    locatie.Adres) },
                                 Array.Empty<Werkingsgebied>()))
                      .ReturnsAsync(GeotagsCollection.Hydrate(geotags));

        var command = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand(
            VerenigingsNaam.Create(naam),
            KorteNaam: null,
            KorteBeschrijving: null,
            Startdatum: null,
            Doelgroep.Null,
            IsUitgeschrevenUitPubliekeDatastroom: false,
            Array.Empty<Contactgegeven>(),
            new[]
            {
                locatie,
            },
            Array.Empty<Vertegenwoordiger>(),
            Array.Empty<HoofdactiviteitVerenigingsloket>(),
            Array.Empty<Werkingsgebied>());

        var commandMetadata = fixture.Create<CommandMetadata>();




        var commandHandler =
            new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(verenigingRepositoryMock,
                                                                                   vCodeService,
                                                                                   new NoDuplicateVerenigingDetectionService(),
                                                                                   martenOutbox.Object,
                                                                                   Mock.Of<IDocumentSession>(),
                                                                                   clock,
                                                                                   grarClient.Object,
                                                                                   geotagsService.Object,
                                                                                   NullLogger<
                                                                                           RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>
                                                                                      .Instance);

        commandHandler
           .Handle(new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, commandMetadata),
                   CancellationToken.None)
           .GetAwaiter()
           .GetResult();

        var vCode = vCodeService.GetLast();

        verenigingRepositoryMock.ShouldHaveSavedExact(
            new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                vCode,
                naam,
                string.Empty,
                string.Empty,
                Startdatum: null,
                EventFactory.Doelgroep(Doelgroep.Null),
                IsUitgeschrevenUitPubliekeDatastroom: false,
                Array.Empty<Registratiedata.Contactgegeven>(),
                new[] { EventFactory.Locatie(locatie) with { LocatieId = Locatie.IdNotSet + 1} },
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()),
            new AdresWerdOvergenomenUitAdressenregister(vCode, LocatieId: Locatie.IdNotSet + 1, adresDetailResponse.AdresId,
                                                        adresDetailResponse.ToAdresUitAdressenregister()),
            new GeotagsWerdenBepaald(vCode, [new Registratiedata.Geotag(geotag.Identificatie)])
    );

    martenOutbox.Verify(expression: v => v.SendAsync(It.IsAny<TeAdresMatchenLocatieMessage>(), It.IsAny<DeliveryOptions>()),
                            Times.Never);
    }
}
