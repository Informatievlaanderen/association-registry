namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events.Factories;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;
using Wolverine.Marten;
using Xunit;

public class With_Locatie_With_AdresId
{
    [Fact]
    public void Then_it_saves_the_event()
    {
        var verenigingRepositoryMock = new NewAggregateSessionMock();
        var vCodeService = new InMemorySequentialVCodeService();
        const string naam = "De sjiekste club";

        var martenOutbox = new Mock<IMartenOutbox>();

        var fixture = new Fixture().CustomizeAdminApi();
        var today = fixture.Create<DateOnly>();

        var clock = new ClockStub(now: today);

        var locatie = fixture.Create<Locatie>() with
        {
            LocatieId = Locatie.IdNotSet, // user does not pass this in, so we set it to 0
            AdresId = fixture.Create<AdresId>(),
        };

        var verrijktAdresUitGrar = new VerrijkteAdressenUitGrar(
            adresWithBronwaarde: new Dictionary<string, Adres>
            {
                { locatie.AdresId.Bronwaarde, fixture.Create<Adres>() },
            }
        );

        var geotag = new Geotag(Identificatie: "BE32");
        var geotags = new[] { geotag };

        var geotagsService = new Mock<IGeotagsService>();
        geotagsService
            .Setup(expression: x =>
                x.CalculateGeotags(
                    new[]
                    {
                        Locatie.Create(
                            Locatienaam.Create(locatie.Naam),
                            locatie.IsPrimair,
                            locatie.Locatietype,
                            AdresId.Create(locatie.AdresId.Adresbron.Code, locatie.AdresId.Bronwaarde),
                            locatie.Adres
                        ),
                    },
                    Array.Empty<Werkingsgebied>()
                )
            )
            .ReturnsAsync(value: GeotagsCollection.Hydrate(geotags: geotags));

        var command = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand(
            OriginalRequest: fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>(),
            Naam: VerenigingsNaam.Create(naam: naam),
            KorteNaam: null,
            KorteBeschrijving: null,
            Startdatum: null,
            Doelgroep: Doelgroep.Null,
            IsUitgeschrevenUitPubliekeDatastroom: false,
            Contactgegevens: [],
            Locaties: [locatie],
            Vertegenwoordigers: [],
            HoofdactiviteitenVerenigingsloket: [],
            Werkingsgebieden: [],
            Bankrekeningnummers: []
        );

        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            newAggregateSession: verenigingRepositoryMock,
            vCodeService: vCodeService,
            outbox: martenOutbox.Object,
            session: Mock.Of<IDocumentSession>(),
            clock: clock,
            geotagsService: geotagsService.Object,
            logger: NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance
        );

        commandHandler
            .Handle(
                message: new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                    Command: command,
                    Metadata: commandMetadata
                ),
                verrijkteAdressenUitGrar: verrijktAdresUitGrar,
                potentialDuplicates: PotentialDuplicatesFound.None,
                personenUitKsz: new PersonenUitKszStub(command: command),
                cancellationToken: CancellationToken.None
            )
            .GetAwaiter()
            .GetResult();

        var vCode = vCodeService.GetLast();

        verenigingRepositoryMock.ShouldHaveSavedExact(
            events:
            [
                new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                    VCode: vCode,
                    Naam: naam,
                    KorteNaam: string.Empty,
                    KorteBeschrijving: string.Empty,
                    Startdatum: null,
                    Doelgroep: EventFactory.Doelgroep(doelgroep: Doelgroep.Null),
                    IsUitgeschrevenUitPubliekeDatastroom: false,
                    Contactgegevens: [],
                    Locaties: [EventFactory.Locatie(locatie: locatie) with { LocatieId = Locatie.IdNotSet + 1 }],
                    Vertegenwoordigers: [],
                    HoofdactiviteitenVerenigingsloket: [],
                    Bankrekeningnummers: [],
                    DuplicatieInfo: Registratiedata.DuplicatieInfo.GeenDuplicaten
                ),
                new AdresWerdOvergenomenUitAdressenregister(
                    VCode: vCode,
                    LocatieId: Locatie.IdNotSet + 1,
                    AdresId: Registratiedata.AdresId.FromAdresId(adres: locatie.AdresId),
                    Adres: Registratiedata.AdresUitAdressenregister.FromAdres(
                        adres: verrijktAdresUitGrar[key: locatie.AdresId.Bronwaarde]
                    )
                ),
                new GeotagsWerdenBepaald(
                    VCode: vCode,
                    Geotags: [new Registratiedata.Geotag(Identificiatie: geotag.Identificatie)]
                ),
            ]
        );

        martenOutbox.Verify(
            expression: v => v.SendAsync(It.IsAny<ProbeerAdresTeMatchenCommand>(), It.IsAny<DeliveryOptions>()),
            times: Times.Never
        );
    }
}
