﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.EventFactories;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DuplicateVerenigingDetection;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging.Geotags;
using Wolverine.Marten;
using Xunit;

public class With_Required_Fields_And_UitgeschrevenUitPubliekeDatastroom
{
    private const string Naam = "naam1";
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public With_Required_Fields_And_UitgeschrevenUitPubliekeDatastroom()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();
        var geotagService = Faktory.New(fixture).GeotagsService.ReturnsEmptyGeotags();

        var today = fixture.Create<DateOnly>();

        var clock = new ClockStub(today);

        var command = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand(
            VerenigingsNaam.Create(Naam),
            KorteNaam: null,
            KorteBeschrijving: null,
            Startdatum: null,
            Doelgroep.Null,
            IsUitgeschrevenUitPubliekeDatastroom: true,
            Array.Empty<Contactgegeven>(),
            Array.Empty<Locatie>(),
            Array.Empty<Vertegenwoordiger>(),
            Array.Empty<HoofdactiviteitVerenigingsloket>(),
            Array.Empty<Werkingsgebied>());

        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler =
            new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(_verenigingRepositoryMock,
                                                             _vCodeService,
                                                             Mock.Of<IMartenOutbox>(),
                                                             Mock.Of<IDocumentSession>(),
                                                             clock,                                                             geotagService.Object,
                                                             NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance);

        commandHandler
           .Handle(
                new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, commandMetadata),
                VerrijkteAdressenUitGrar.Empty,
                PotentialDuplicatesFound.None,
                CancellationToken.None)
           .GetAwaiter()
           .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        var vCode = _vCodeService.GetLast();

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new  VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                vCode,
                Naam,
                string.Empty,
                string.Empty,
                Startdatum: null,
                EventFactory.Doelgroep(Doelgroep.Null),
                IsUitgeschrevenUitPubliekeDatastroom: true,
                Array.Empty<Registratiedata.Contactgegeven>(),
                Array.Empty<Registratiedata.Locatie>(),
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()), new GeotagsWerdenBepaald(vCode, []));
    }
}
