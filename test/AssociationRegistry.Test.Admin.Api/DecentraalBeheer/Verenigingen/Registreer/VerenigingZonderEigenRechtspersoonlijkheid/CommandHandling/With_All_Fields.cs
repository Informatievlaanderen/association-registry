﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.EventFactories;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AutoFixture;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_All_Fields
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand _command;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public With_All_Fields()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();

        _command = fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();
        var clock = new ClockStub(_command.Startdatum.Value);

        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler =
            new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(_verenigingRepositoryMock,
                                                             _vCodeService,
                                                             new NoDuplicateVerenigingDetectionService(),
                                                             Mock.Of<IMartenOutbox>(),
                                                             Mock.Of<IDocumentSession>(),
                                                             clock,
                                                             Mock.Of<IGrarClient>(),
                                                             NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance);

        commandHandler
           .Handle(new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(_command, commandMetadata), CancellationToken.None)
           .GetAwaiter()
           .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        var vCode = _vCodeService.GetLast();

        var  VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = new  VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
            vCode,
            _command.Naam,
            _command.KorteNaam ?? string.Empty,
            _command.KorteBeschrijving ?? string.Empty,
            _command.Startdatum,
            EventFactory.Doelgroep(_command.Doelgroep),
            _command.IsUitgeschrevenUitPubliekeDatastroom,
            _command.Contactgegevens.Select(
                (c, i) =>
                    new Registratiedata.Contactgegeven(
                        i + 1,
                        c.Contactgegeventype,
                        c.Waarde,
                        c.Beschrijving,
                        c.IsPrimair
                    )).ToArray(),
            _command.Locaties.Select(
                (l, i) =>
                    new Registratiedata.Locatie(
                        i + 1,
                        l.Locatietype,
                        l.IsPrimair,
                        l.Naam ?? string.Empty,
                        new Registratiedata.Adres(l.Adres!.Straatnaam,
                                                  l.Adres.Huisnummer,
                                                  l.Adres.Busnummer,
                                                  l.Adres.Postcode,
                                                  l.Adres.Gemeente.Naam,
                                                  l.Adres.Land),
                        AdresId: null)
            ).ToArray(),
            _command.Vertegenwoordigers.Select(
                (v, i) =>
                    new Registratiedata.Vertegenwoordiger(
                        i + 1,
                        v.Insz,
                        v.IsPrimair,
                        v.Roepnaam ?? string.Empty,
                        v.Rol ?? string.Empty,
                        v.Voornaam,
                        v.Achternaam,
                        v.Email.Waarde,
                        v.Telefoon.Waarde,
                        v.Mobiel.Waarde,
                        v.SocialMedia.Waarde
                    )).ToArray(),
            _command.HoofdactiviteitenVerenigingsloket.Select(
                h =>
                    new Registratiedata.HoofdactiviteitVerenigingsloket(h.Code, h.Naam)
            ).ToArray());

        var werkingsgebiedenWerdenBepaald = new WerkingsgebiedenWerdenBepaald(
            vCode,
            _command.Werkingsgebieden.Select(h => new Registratiedata.Werkingsgebied(h.Code, h.Naam)).ToArray());

        _verenigingRepositoryMock.ShouldHaveSaved(
             VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            werkingsgebiedenWerdenBepaald
        );
    }
}
