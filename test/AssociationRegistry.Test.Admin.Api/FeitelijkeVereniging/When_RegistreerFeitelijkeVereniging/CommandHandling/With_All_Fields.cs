﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.CommandHandling;

using Acties.RegistreerFeitelijkeVereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_All_Fields
{
    private readonly RegistreerFeitelijkeVerenigingCommand _command;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public With_All_Fields()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();

        _command = fixture.Create<RegistreerFeitelijkeVerenigingCommand>();
        var clock = new ClockStub(_command.Startdatum.Value);

        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler =
            new RegistreerFeitelijkeVerenigingCommandHandler(_verenigingRepositoryMock, _vCodeService,
                                                             new NoDuplicateVerenigingDetectionService(), clock);

        commandHandler
           .Handle(new CommandEnvelope<RegistreerFeitelijkeVerenigingCommand>(_command, commandMetadata), CancellationToken.None)
           .GetAwaiter()
           .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new FeitelijkeVerenigingWerdGeregistreerd(
                _vCodeService.GetLast(),
                _command.Naam,
                _command.KorteNaam ?? string.Empty,
                _command.KorteBeschrijving ?? string.Empty,
                _command.Startdatum,
                Registratiedata.Doelgroep.With(_command.Doelgroep),
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
                                                      l.Adres.Gemeente,
                                                      l.Adres.Land),
                            new Registratiedata.AdresId(l.AdresId!.Adresbron.Code, l.AdresId.Bronwaarde))
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
                ).ToArray()));
    }
}
