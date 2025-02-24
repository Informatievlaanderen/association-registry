﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.When_Registreer.CommandHandling;

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
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Locatie_With_AdresId
{
    [Fact]
    public void Then_it_saves_the_event()
    {
        var verenigingRepositoryMock = new VerenigingRepositoryMock();
        var vCodeService = new InMemorySequentialVCodeService();
        const string naam = "De sjiekste club";

        var grarClient = new Mock<IGrarClient>();
        var martenOutbox = new Mock<IMartenOutbox>();

        var fixture = new Fixture().CustomizeAdminApi();
        var today = fixture.Create<DateOnly>();

        var clock = new ClockStub(today);

        var locatie = fixture.Create<Locatie>() with
        {
            LocatieId = 1,
            AdresId = fixture.Create<AdresId>(),
        };

        var adresDetailResponse = fixture.Create<AddressDetailResponse>() with
        {
            AdresId = new Registratiedata.AdresId(locatie.AdresId.Adresbron.Code, locatie.AdresId.Bronwaarde),
            IsActief = true,
        };

        grarClient.Setup(s => s.GetAddressById(locatie.AdresId.ToString(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(adresDetailResponse);

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
                                                             NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance);

        commandHandler
           .Handle(new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, commandMetadata), CancellationToken.None)
           .GetAwaiter()
           .GetResult();

        verenigingRepositoryMock.ShouldHaveSaved(
            new  VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                vCodeService.GetLast(),
                naam,
                string.Empty,
                string.Empty,
                Startdatum: null,
                EventFactory.Doelgroep(Doelgroep.Null),
                IsUitgeschrevenUitPubliekeDatastroom: false,
                Array.Empty<Registratiedata.Contactgegeven>(),
                new[] { EventFactory.Locatie(locatie) },
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()),
            new AdresWerdOvergenomenUitAdressenregister(vCodeService.GetLast(), LocatieId: 1, adresDetailResponse.AdresId,
                                                        adresDetailResponse.ToAdresUitAdressenregister())
        );

        martenOutbox.Verify(expression: v => v.SendAsync(It.IsAny<TeAdresMatchenLocatieMessage>(), It.IsAny<DeliveryOptions>()),
                            Times.Never);
    }
}
