﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.
    CommandHandling;

using AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.EventFactories;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Messages;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DuplicateVerenigingDetection;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging.Geotags;
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
                vCode,
                naam,
                string.Empty,
                string.Empty,
                Startdatum: null,
                EventFactory.Doelgroep(Doelgroep.Null),
                IsUitgeschrevenUitPubliekeDatastroom: false,
                [],
                [EventFactory.Locatie(locatie) with { LocatieId = Locatie.IdNotSet + 1}],
                [],
                []),
            new AdresWerdOvergenomenUitAdressenregister(vCode, LocatieId: Locatie.IdNotSet + 1,
                                                        Registratiedata.AdresId.FromAdresId(locatie.AdresId),
                                                        Registratiedata.AdresUitAdressenregister.FromAdres(verrijktAdresUitGrar[locatie.AdresId.Bronwaarde])),
            new GeotagsWerdenBepaald(vCode, [new Registratiedata.Geotag(geotag.Identificatie)])
    );

    martenOutbox.Verify(expression: v => v.SendAsync(It.IsAny<TeAdresMatchenLocatieMessage>(), It.IsAny<DeliveryOptions>()),
                            Times.Never);
    }
}
