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

public class With_Two_Primair_Contactgegevens_Of_Different_Type : IAsyncLifetime
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand _command;
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler _commandHandler;
    private readonly IFixture _fixture;
    private readonly VerenigingRepositoryMock _repositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;

    public With_Two_Primair_Contactgegevens_Of_Different_Type()
    {
        _fixture = new Fixture().CustomizeAdminApi()
                                .WithoutWerkingsgebieden();

        var geotagService = Faktory.New(_fixture).GeotagsService.ReturnsEmptyGeotags();

        _repositoryMock = new VerenigingRepositoryMock();

        _command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Contactgegevens = new[]
            {
                Contactgegeven.CreateFromInitiator(
                    Contactgegeventype.Email,
                    waarde: "test@example.org",
                    beschrijving: _fixture.Create<string>(),
                    isPrimair: true),

                Contactgegeven.CreateFromInitiator(
                    Contactgegeventype.Website,
                    waarde: "http://example.org",
                    beschrijving: _fixture.Create<string>(),
                    isPrimair: true),
            },
        };

        _vCodeService = new InMemorySequentialVCodeService();

        _commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            _repositoryMock,
            _vCodeService,
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>(),
            new ClockStub(_command.Startdatum.Value),
            geotagService.Object,
            NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance);
    }

    public async ValueTask InitializeAsync()
    {
        var commandMetadata = _fixture.Create<CommandMetadata>();

        await _commandHandler.Handle(
            new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                _command,
                commandMetadata),
            VerrijkteAdressenUitGrar.Empty,
            PotentialDuplicatesFound.None,
            CancellationToken.None);
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;

    [Fact]
    public void Then_it_saves_the_event()
    {
        var vCode = _vCodeService.GetLast();

        _repositoryMock.ShouldHaveSavedExact(
            new  VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                vCode,
                _command.Naam,
                _command.KorteNaam ?? string.Empty,
                _command.KorteBeschrijving ?? string.Empty,
                _command.Startdatum,
                EventFactory.Doelgroep(_command.Doelgroep),
                _command.IsUitgeschrevenUitPubliekeDatastroom,
                new[]
                {
                    new Registratiedata.Contactgegeven(
                        ContactgegevenId: 1,
                        Contactgegeventype.Email,
                        _command.Contactgegevens[0].Waarde,
                        _command.Contactgegevens[0].Beschrijving,
                        _command.Contactgegevens[0].IsPrimair
                    ),
                    new Registratiedata.Contactgegeven(
                        ContactgegevenId: 2,
                        Contactgegeventype.Website,
                        _command.Contactgegevens[1].Waarde,
                        _command.Contactgegevens[1].Beschrijving,
                        _command.Contactgegevens[1].IsPrimair
                    ),
                },
                _command.Locaties.Select(
                    (l, index) => EventFactory.Locatie(l) with
                    {
                        LocatieId = index + 1,
                    }).ToArray(),
                _command.Vertegenwoordigers.Select(
                    (v, index) => EventFactory.Vertegenwoordiger(v) with
                    {
                        VertegenwoordigerId = index + 1,
                    }).ToArray(),
                _command.HoofdactiviteitenVerenigingsloket.Select(
                    h => new Registratiedata.HoofdactiviteitVerenigingsloket(
                        h.Code,
                        h.Naam)).ToArray()
            ), new GeotagsWerdenBepaald(vCode, []));
    }
}
