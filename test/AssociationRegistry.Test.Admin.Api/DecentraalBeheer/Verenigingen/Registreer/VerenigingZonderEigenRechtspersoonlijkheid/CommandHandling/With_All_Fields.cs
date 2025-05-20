namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.EventFactories;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AutoFixture;
using Common.Stubs.VCodeServices;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Vereniging.Geotags;
using Wolverine.Marten;
using Xunit;

public class With_All_Fields
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand _command;
    private readonly StubVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private GeoTag[] _geotags;
    private ClockStub _clock;
    private Fixture _fixture;
    private Mock<IGeotagsService> _geotagsService;

    public With_All_Fields()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        var vCode = _fixture.Create<VCode>();

        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new StubVCodeService(vCode);
        _clock = new ClockStub(_fixture.Create<DateOnly>());
        _geotagsService = new Mock<IGeotagsService>();

        _geotags = _fixture.CreateMany<GeoTag>().ToArray();

        _command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>()
            with { Startdatum = Datum.Hydrate(_clock.Today.AddDays(-1))};

        _geotagsService.Setup(x => x.CalculateGeotags(_command.Locaties))
                       .ReturnsAsync(_geotags);
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var commandHandler =
            new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(_verenigingRepositoryMock,
                                                                                   _vCodeService,
                                                                                   new NoDuplicateVerenigingDetectionService(),
                                                                                   Mock.Of<IMartenOutbox>(),
                                                                                   Mock.Of<IDocumentSession>(),
                                                                                   _clock,
                                                                                   Mock.Of<IGrarClient>(),
                                                                                   _geotagsService.Object,
                                                                                   NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance);

        commandHandler
                       .Handle(new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(_command, commandMetadata), CancellationToken.None)
                       .GetAwaiter()
                       .GetResult();

        var verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = new  VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
            _vCodeService.VCode,
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
            _vCodeService.VCode,
            _command.Werkingsgebieden.Select(h => new Registratiedata.Werkingsgebied(h.Code, h.Naam)).ToArray());

        var geotagsWerdenBepaald =
            new GeotagsWerdenBepaald(_vCodeService.VCode, _geotags.Select(x => new Registratiedata.Geotag(x.Identificatie)).ToArray());

        _verenigingRepositoryMock.ShouldHaveSaved(
             verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            werkingsgebiedenWerdenBepaald,
            geotagsWerdenBepaald
        );
    }
}
