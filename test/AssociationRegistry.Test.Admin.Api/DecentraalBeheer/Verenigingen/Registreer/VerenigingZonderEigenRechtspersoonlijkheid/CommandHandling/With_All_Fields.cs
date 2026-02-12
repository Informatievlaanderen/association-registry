namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events.Factories;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Wolverine.Marten;
using Xunit;

public class With_All_Fields
{
    private readonly StubVCodeService _vCodeService;
    private readonly NewAggregateSessionMock _newAggregateSessionMock;
    private readonly ClockStub _clock;
    private readonly Fixture _fixture;
    private readonly Mock<IGeotagsService> _geotagsService;
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand _command;
    private GeotagsCollection _geotags;

    public With_All_Fields()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        var faktory = Faktory.New(fixture: _fixture);
        var today = _fixture.Create<DateOnly>();

        _command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Startdatum = Datum.Hydrate(dateOnly: today.AddDays(value: -1)),
        };

        _newAggregateSessionMock = faktory.NewAggregateSession.Mock();
        _vCodeService = faktory.VCodeService.Stub(vCode: _fixture.Create<VCode>());
        (_geotagsService, _geotags) = faktory.GeotagsService.ReturnsRandomGeotags(
            givenLocaties: _command.Locaties,
            givenWerkingsgebieden: _command.Werkingsgebieden
        );
        _clock = faktory.Clock.Stub(date: today);
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            newAggregateSession: _newAggregateSessionMock,
            vCodeService: _vCodeService,
            outbox: Mock.Of<IMartenOutbox>(),
            session: Mock.Of<IDocumentSession>(),
            clock: _clock,
            geotagsService: _geotagsService.Object,
            logger: NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance
        );

        commandHandler
            .Handle(
                message: new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                    Command: _command,
                    Metadata: commandMetadata
                ),
                verrijkteAdressenUitGrar: VerrijkteAdressenUitGrar.Empty,
                potentialDuplicates: PotentialDuplicatesFound.None,
                personenUitKsz: new PersonenUitKszStub(command: _command),
                cancellationToken: CancellationToken.None
            )
            .GetAwaiter()
            .GetResult();

        var verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                VCode: _vCodeService.VCode,
                Naam: _command.Naam,
                KorteNaam: _command.KorteNaam ?? string.Empty,
                KorteBeschrijving: _command.KorteBeschrijving ?? string.Empty,
                Startdatum: _command.Startdatum,
                Doelgroep: EventFactory.Doelgroep(doelgroep: _command.Doelgroep),
                IsUitgeschrevenUitPubliekeDatastroom: _command.IsUitgeschrevenUitPubliekeDatastroom,
                Contactgegevens: _command
                    .Contactgegevens.Select(
                        selector: (c, i) =>
                            new Registratiedata.Contactgegeven(
                                ContactgegevenId: i + 1,
                                Contactgegeventype: c.Contactgegeventype,
                                Waarde: c.Waarde,
                                Beschrijving: c.Beschrijving,
                                IsPrimair: c.IsPrimair
                            )
                    )
                    .ToArray(),
                Locaties: _command
                    .Locaties.Select(
                        selector: (l, i) =>
                            new Registratiedata.Locatie(
                                LocatieId: i + 1,
                                Locatietype: l.Locatietype,
                                IsPrimair: l.IsPrimair,
                                Naam: l.Naam ?? string.Empty,
                                Adres: new Registratiedata.Adres(
                                    Straatnaam: l.Adres!.Straatnaam,
                                    Huisnummer: l.Adres.Huisnummer,
                                    Busnummer: l.Adres.Busnummer,
                                    Postcode: l.Adres.Postcode,
                                    Gemeente: l.Adres.Gemeente.Naam,
                                    Land: l.Adres.Land
                                ),
                                AdresId: null
                            )
                    )
                    .ToArray(),
                Vertegenwoordigers: _command
                    .Vertegenwoordigers.Select(
                        selector: (v, i) =>
                            new Registratiedata.Vertegenwoordiger(
                                VertegenwoordigerId: i + 1,
                                Insz: v.Insz,
                                IsPrimair: v.IsPrimair,
                                Roepnaam: v.Roepnaam ?? string.Empty,
                                Rol: v.Rol ?? string.Empty,
                                Voornaam: v.Voornaam,
                                Achternaam: v.Achternaam,
                                Email: v.Email.Waarde,
                                Telefoon: v.Telefoon.Waarde,
                                Mobiel: v.Mobiel.Waarde,
                                SocialMedia: v.SocialMedia.Waarde
                            )
                    )
                    .ToArray(),
                HoofdactiviteitenVerenigingsloket: _command
                    .HoofdactiviteitenVerenigingsloket.Select(
                        selector: h => new Registratiedata.HoofdactiviteitVerenigingsloket(Code: h.Code, Naam: h.Naam)
                    )
                    .ToArray(),
                Bankrekeningnummers: _command
                    .Bankrekeningnummers.Select(
                        selector: (x, i) =>
                            new Registratiedata.Bankrekeningnummer(
                                BankrekeningnummerId: ++i,
                                Iban: x.Iban.Value,
                                Doel: x.Doel,
                                Titularis: x.Titularis.Value
                            )
                    )
                    .ToArray(),
                DuplicatieInfo: Registratiedata.DuplicatieInfo.GeenDuplicaten
            );

        var werkingsgebiedenWerdenBepaald = new WerkingsgebiedenWerdenBepaald(
            VCode: _vCodeService.VCode,
            Werkingsgebieden: _command
                .Werkingsgebieden.Select(selector: h => new Registratiedata.Werkingsgebied(Code: h.Code, Naam: h.Naam))
                .ToArray()
        );

        var geotagsWerdenBepaald = new GeotagsWerdenBepaald(
            VCode: _vCodeService.VCode,
            Geotags: _geotags
                .Select(selector: x => new Registratiedata.Geotag(Identificiatie: x.Identificatie))
                .ToArray()
        );

        _newAggregateSessionMock.ShouldHaveSavedExact(
            events:
            [
                verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                werkingsgebiedenWerdenBepaald,
                geotagsWerdenBepaald,
            ]
        );
    }
}
