namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequestMapping;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Emails;
using AssociationRegistry.DecentraalBeheer.Vereniging.SocialMedias;
using AssociationRegistry.DecentraalBeheer.Vereniging.TelefoonNummers;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Framework;
using FluentAssertions;
using Xunit;

public class To_A_RegistreerFeitelijkeVerenigingCommand
{
    [Fact]
    public void Then_We_Get_A_RegistreerFeitelijkeVerenigingCommand()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<RegistreerFeitelijkeVerenigingRequest>();

        IWerkingsgebiedenService werkingsgebiedenService = new WerkingsgebiedenServiceMock();
        var actual = request.ToCommand(
            werkingsgebieden: request
                .Werkingsgebieden?.Select(selector: s => werkingsgebiedenService.Create(code: s))
                .ToArray()
        );

        actual.Deconstruct(
            OriginalRequest: out var originalRequest,
            Naam: out var naam,
            KorteNaam: out var korteNaam,
            KorteBeschrijving: out var korteBeschrijving,
            Startdatum: out var startdatum,
            Doelgroep: out var doelgroep,
            IsUitgeschrevenUitPubliekeDatastroom: out var isUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens: out var contactgegevens,
            Locaties: out var locaties,
            Vertegenwoordigers: out var vertegenwoordigers,
            HoofdactiviteitenVerenigingsloket: out var hoofdactiviteiten,
            Werkingsgebieden: out var werkingsgebieden,
            Bankrekeningnummers: out var bankrekeningnummers,
            Bevestigingstoken: out var bevestigingstoken
        );

        originalRequest.Should().Be(expected: request);
        naam.ToString().Should().Be(expected: request.Naam);
        korteNaam.Should().Be(expected: request.KorteNaam);
        korteBeschrijving.Should().Be(expected: request.KorteBeschrijving);
        ((DateOnly?)startdatum).Should().Be(expected: request.Startdatum);
        doelgroep
            .Should()
            .BeEquivalentTo(
                expectation: Doelgroep.Create(
                    minimumleeftijd: request.Doelgroep!.Minimumleeftijd,
                    maximumleeftijd: request.Doelgroep.Maximumleeftijd
                )
            );
        isUitgeschrevenUitPubliekeDatastroom.Should().Be(expected: request.IsUitgeschrevenUitPubliekeDatastroom);

        AssertContactgegevens(contactgegevens: contactgegevens, request: request);
        AssertLocaties(locaties: locaties, request: request);
        AssertVertegenwoordigers(vertegenwoordigers: vertegenwoordigers, request: request);

        hoofdactiviteiten
            .Select(selector: x => x.Code)
            .Should()
            .BeEquivalentTo(expectation: request.HoofdactiviteitenVerenigingsloket);
        werkingsgebieden.Select(selector: x => x.Code).Should().BeEquivalentTo(expectation: request.Werkingsgebieden);
        bankrekeningnummers
            .Select(selector: x => new
            {
                Iban = x.Iban.Value,
                Doel = x.Doel,
                Titularis = x.Titularis.Value,
            })
            .Should()
            .BeEquivalentTo(
                expectation: request.Bankrekeningnummers.Select(selector: x => new
                {
                    Iban = x.Iban,
                    Doel = x.Doel,
                    Titularis = x.Titularis,
                })
            );
        bevestigingstoken.Should().BeEmpty();
    }

    private static void AssertVertegenwoordigers(
        Vertegenwoordiger[] vertegenwoordigers,
        RegistreerFeitelijkeVerenigingRequest request
    )
    {
        vertegenwoordigers
            .Should()
            .BeEquivalentTo(
                expectation: request.Vertegenwoordigers.Select(selector: v =>
                    Vertegenwoordiger.Create(
                        insz: Insz.Create(insz: v.Insz),
                        primairContactpersoon: v.IsPrimair,
                        roepnaam: v.Roepnaam,
                        rol: v.Rol,
                        voornaam: Voornaam.Create(waarde: v.Voornaam),
                        achternaam: Achternaam.Create(waarde: v.Achternaam),
                        email: Email.Create(email: v.Email),
                        telefoonNummer: TelefoonNummer.Create(telefoonNummer: v.Telefoon),
                        mobiel: TelefoonNummer.Create(telefoonNummer: v.Mobiel),
                        socialMedia: SocialMedia.Create(socialMedia: v.SocialMedia)
                    )
                )
            );
    }

    private static void AssertLocaties(Locatie[] locaties, RegistreerFeitelijkeVerenigingRequest request)
    {
        foreach (var (locatie, i) in locaties.Select(selector: (l, i) => (l, i)))
        {
            AssertLocatie(locatie: locatie, requestLocatie: request.Locaties[i]);
        }
    }

    private static void AssertLocatie(Locatie locatie, ToeTeVoegenLocatie requestLocatie)
    {
        locatie.Locatietype.Waarde.Should().Be(expected: requestLocatie.Locatietype);
        locatie.Naam.Should().Be(expected: requestLocatie.Naam);
        locatie.IsPrimair.Should().Be(expected: requestLocatie.IsPrimair);
        locatie.Adres!.Straatnaam.Should().Be(expected: requestLocatie.Adres!.Straatnaam);
        locatie.Adres!.Huisnummer.Should().Be(expected: requestLocatie.Adres!.Huisnummer);
        locatie.Adres!.Busnummer.Should().Be(expected: requestLocatie.Adres!.Busnummer);
        locatie.Adres!.Postcode.Should().Be(expected: requestLocatie.Adres!.Postcode);
        locatie.Adres!.Gemeente.Naam.Should().Be(expected: requestLocatie.Adres!.Gemeente);
        locatie.Adres!.Land.Should().Be(expected: requestLocatie.Adres!.Land);
    }

    private static void AssertContactgegevens(
        Contactgegeven[] contactgegevens,
        RegistreerFeitelijkeVerenigingRequest request
    )
    {
        contactgegevens[0]
            .Should()
            .BeEquivalentTo(
                expectation: Contactgegeven.CreateFromInitiator(
                    type: Contactgegeventype.Parse(value: request.Contactgegevens[0].Contactgegeventype),
                    waarde: request.Contactgegevens[0].Waarde,
                    beschrijving: request.Contactgegevens[0].Beschrijving,
                    isPrimair: request.Contactgegevens[0].IsPrimair
                )
            );
    }
}
