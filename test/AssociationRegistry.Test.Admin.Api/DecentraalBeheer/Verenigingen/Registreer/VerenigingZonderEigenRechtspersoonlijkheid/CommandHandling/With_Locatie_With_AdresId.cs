namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.
    CommandHandling;

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
        var verenigingRepositoryMock = new VerenigingRepositoryMock();
        var vCodeService = new InMemorySequentialVCodeService();
        const string naam = "De sjiekste club";

        var martenOutbox = new Mock<IMartenOutbox>();

        var fixture = new Fixture().CustomizeAdminApi();
        var today = fixture.Create<DateOnly>();

        var clock = new ClockStub(today);

        var locatie = fixture.Create<Locatie>() with
        {
            LocatieId = Locatie.IdNotSet, // user does not pass this in, so we set it to 0
            AdresId = fixture.Create<AdresId>(),
        };

        var verrijktAdresUitGrar = new VerrijkteAdressenUitGrar(new Dictionary<string, Adres>
        {
            { locatie.AdresId.Bronwaarde, fixture.Create<Adres>() }
        });

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
            Naam: VerenigingsNaam.Create(naam),
            KorteNaam: null,
            KorteBeschrijving: null,
            Startdatum: null,
            Doelgroep: Doelgroep.Null,
            IsUitgeschrevenUitPubliekeDatastroom: false,
            Contactgegevens: [],
            Locaties:
            [
                locatie,
            ],
            Vertegenwoordigers: [],
            HoofdactiviteitenVerenigingsloket: [],
            Werkingsgebieden: []);

        var commandMetadata = fixture.Create<CommandMetadata>();



        var commandHandler =
            new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(verenigingRepositoryMock,
                                                                                   vCodeService,
                                                                                   martenOutbox.Object,
                                                                                   Mock.Of<IDocumentSession>(),
                                                                                   clock,
                                                                                   geotagsService.Object,
                                                                                   NullLogger<
                                                                                           RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>
                                                                                      .Instance);

        commandHandler
           .Handle(
                new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, commandMetadata),
                verrijktAdresUitGrar,
                PotentialDuplicatesFound.None,
                CancellationToken.None)
           .GetAwaiter()
           .GetResult();

        var vCode = vCodeService.GetLast();

        verenigingRepositoryMock.ShouldHaveSavedExact(
            new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                VCode: vCode,
                Naam: naam,
                KorteNaam: string.Empty,
                KorteBeschrijving: string.Empty,
                Startdatum: null,
                Doelgroep: EventFactory.Doelgroep(Doelgroep.Null),
                IsUitgeschrevenUitPubliekeDatastroom: false,
                Contactgegevens: [],
                Locaties: [EventFactory.Locatie(locatie) with { LocatieId = Locatie.IdNotSet + 1}],
                Vertegenwoordigers: [],
                HoofdactiviteitenVerenigingsloket: [],
                Registratiedata.DuplicatieInfo.GeenDuplicaten
            ),
            new AdresWerdOvergenomenUitAdressenregister(vCode, LocatieId: Locatie.IdNotSet + 1,
                                                        Registratiedata.AdresId.FromAdresId(locatie.AdresId),
                                                        Registratiedata.AdresUitAdressenregister.FromAdres(verrijktAdresUitGrar[locatie.AdresId.Bronwaarde])),
            new GeotagsWerdenBepaald(vCode, [new Registratiedata.Geotag(geotag.Identificatie)])
    );

    martenOutbox.Verify(expression: v => v.SendAsync(It.IsAny<ProbeerAdresTeMatchenCommand>(), It.IsAny<DeliveryOptions>()),
                            Times.Never);
    }
}
