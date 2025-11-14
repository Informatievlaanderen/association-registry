namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

using AssociationRegistry.Admin.Schema.Persoonsgegevens;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AutoFixture;
using Events;
using Events.Factories;
using FluentAssertions;
using Xunit;

public class With_All_Fields : RegistreerVZERCommandHandlerTestBase
{
    protected override RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand CreateCommand()
    {
        var command = Fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();

        return command with { Startdatum = Datum.Hydrate(Today.AddDays(-1)) };
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        var verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                VCodeService.VCode,
                Command.Naam,
                Command.KorteNaam ?? string.Empty,
                Command.KorteBeschrijving ?? string.Empty,
                Command.Startdatum,
                EventFactory.Doelgroep(Command.Doelgroep),
                Command.IsUitgeschrevenUitPubliekeDatastroom,
                Command.Contactgegevens.Select(
                    (c, i) =>
                        new Registratiedata.Contactgegeven(
                            i + 1,
                            c.Contactgegeventype,
                            c.Waarde,
                            c.Beschrijving,
                            c.IsPrimair
                        )).ToArray(),
                Command.Locaties.Select(
                    (l, i) =>
                        new Registratiedata.Locatie(
                            i + 1,
                            l.Locatietype,
                            l.IsPrimair,
                            l.Naam ?? string.Empty,
                            new Registratiedata.Adres(
                                l.Adres!.Straatnaam,
                                l.Adres.Huisnummer,
                                l.Adres.Busnummer,
                                l.Adres.Postcode,
                                l.Adres.Gemeente.Naam,
                                l.Adres.Land),
                            AdresId: null)
                ).ToArray(),
                Command.Vertegenwoordigers.Select(EnrichVertegenwoordiger).ToArray(),
                Command.HoofdactiviteitenVerenigingsloket
                       .Select(h => new Registratiedata.HoofdactiviteitVerenigingsloket(h.Code, h.Naam))
                       .ToArray(),
                Registratiedata.DuplicatieInfo.GeenDuplicaten
            );

        var werkingsgebiedenWerdenBepaald = new WerkingsgebiedenWerdenBepaald(
            VCodeService.VCode,
            Command.Werkingsgebieden
                   .Select(h => new Registratiedata.Werkingsgebied(h.Code, h.Naam))
                   .ToArray());

        var geotagsWerdenBepaald = new GeotagsWerdenBepaald(
            VCodeService.VCode,
            Geotags.Select(x => new Registratiedata.Geotag(x.Identificatie)).ToArray());

        VerenigingRepositoryMock.ShouldHaveSavedExact(
            verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            werkingsgebiedenWerdenBepaald,
            geotagsWerdenBepaald);
    }

    [Fact]
    public void Then_It_Saves_PersoonsGegevens_Foreach_Vertegenwoordiger()
    {
        var expectedPersoonsgegevens = Command.Vertegenwoordigers.Select((vertegenwoordiger, i)
                                                                             => new VertegenwoordigerPersoonsgegevensDocument
                                                                             {
                                                                                 RefId = Guid.Empty,
                                                                                 VCode = VCodeService.VCode,
                                                                                 VertegenwoordigerId = ++i,
                                                                                 Insz = vertegenwoordiger.Insz,
                                                                                 Roepnaam = vertegenwoordiger.Roepnaam,
                                                                                 Rol = vertegenwoordiger.Rol,
                                                                                 Voornaam = vertegenwoordiger.Voornaam,
                                                                                 Achternaam = vertegenwoordiger.Achternaam,
                                                                                 Email = vertegenwoordiger.Email.Waarde,
                                                                                 Telefoon = vertegenwoordiger.Telefoon.Waarde,
                                                                                 Mobiel = vertegenwoordiger.Mobiel.Waarde,
                                                                                 SocialMedia = vertegenwoordiger.SocialMedia.Waarde,
                                                                             });

        var actual = VertegenwoordigerPersoonsgegevensRepositoryMock.GetAll();
        actual.Should().BeEquivalentTo(expectedPersoonsgegevens);
    }

    private Registratiedata.Vertegenwoordiger EnrichVertegenwoordiger(Vertegenwoordiger vertegenwoordiger, int index)
    {
        var vertegenwoordigerId = ++index;
        var persoonsdata = VertegenwoordigerPersoonsgegevensRepositoryMock.FindByVCodeAndVertegenwoordigerId(VCodeService.VCode, vertegenwoordigerId);

        return new Registratiedata.Vertegenwoordiger(persoonsdata!.RefId, vertegenwoordigerId, vertegenwoordiger.IsPrimair);
    }
}
