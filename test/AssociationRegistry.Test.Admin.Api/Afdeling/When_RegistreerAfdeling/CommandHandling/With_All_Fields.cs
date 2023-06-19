namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.CommandHandling;

using Acties.RegistreerAfdeling;
using Events;
using AssociationRegistry.Framework;
using Fakes;
using Framework;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_All_Fields
{
    private readonly RegistreerAfdelingCommand _command;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public With_All_Fields()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAll();

        _command = fixture.Create<RegistreerAfdelingCommand>();
        var clock = new ClockStub(_command.Startdatum.Datum!.Value);

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerAfdelingCommandHandler(
            _verenigingRepositoryMock,
            _vCodeService,
            new NoDuplicateVerenigingDetectionService(),
            clock);

        commandHandler
            .Handle(new CommandEnvelope<RegistreerAfdelingCommand>(_command, commandMetadata), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new AfdelingWerdGeregistreerd(
                _vCodeService.GetLast(),
                _command.Naam,
                new AfdelingWerdGeregistreerd.MoederverenigingsData(
                    _command.KboNummerMoedervereniging,
                    string.Empty,
                    $"Moeder {_command.KboNummerMoedervereniging}"),
                _command.KorteNaam ?? string.Empty,
                _command.KorteBeschrijving ?? string.Empty,
                _command.Startdatum,
                _command.IsUitgeschrevenUitPubliekeDatastroom,
                _command.Contactgegevens.Select(
                    (c, i) =>
                        new Registratiedata.Contactgegeven(
                            i + 1,
                            c.Type,
                            c.Waarde,
                            c.Beschrijving,
                            c.IsPrimair
                        )).ToArray(),
                _command.Locaties.Select(
                    (l, i) =>
                        new Registratiedata.Locatie(
                            i + 1,
                            l.Naam ?? string.Empty,
                            l.Straatnaam,
                            l.Huisnummer,
                            l.Busnummer ?? string.Empty,
                            l.Postcode,
                            l.Gemeente,
                            l.Land,
                            l.Hoofdlocatie,
                            l.Locatietype)
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
                        new Registratiedata.HoofdactiviteitVerenigingsloket(h.Code, h.Beschrijving)
                ).ToArray()));
    }
}
