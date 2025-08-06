namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestMapping;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Emails;
using AssociationRegistry.Vereniging.SocialMedias;
using AssociationRegistry.Vereniging.TelefoonNummers;
using AutoFixture;
using Common.Framework;
using FluentAssertions;
using Xunit;

public class To_A_RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand
{
    [Fact]
    public void Then_We_Get_A_RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>();

        var actual = request.ToCommand(new WerkingsgebiedenServiceMock());

        actual.Deconstruct(
            out var naam,
            out var korteNaam,
            out var korteBeschrijving,
            out var startdatum,
            out var doelgroep,
            out var isUitgeschrevenUitPubliekeDatastroom,
            out var contactgegevens,
            out var locaties,
            out var vertegenwoordigers,
            out var hoofdactiviteiten,
            out var werkingsgebieden,
            out var skipDuplicateDetection);

        naam.ToString().Should().Be(request.Naam);
        korteNaam.Should().Be(request.KorteNaam);
        korteBeschrijving.Should().Be(request.KorteBeschrijving);
        ((DateOnly?)startdatum).Should().Be(request.Startdatum);
        doelgroep.Should().BeEquivalentTo(Doelgroep.Create(request.Doelgroep!.Minimumleeftijd, request.Doelgroep.Maximumleeftijd));
        isUitgeschrevenUitPubliekeDatastroom.Should().Be(request.IsUitgeschrevenUitPubliekeDatastroom);

        AssertContactgegevens(contactgegevens, request);
        AssertLocaties(locaties, request);
        AssertVertegenwoordigers(vertegenwoordigers, request);

        hoofdactiviteiten.Select(x => x.Code).Should().BeEquivalentTo(request.HoofdactiviteitenVerenigingsloket);
        werkingsgebieden.Select(x => x.Code).Should().BeEquivalentTo(request.Werkingsgebieden);
        skipDuplicateDetection.Should().BeFalse();
    }

    private static void AssertVertegenwoordigers(Vertegenwoordiger[] vertegenwoordigers, RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest request)
    {
        vertegenwoordigers.Should().BeEquivalentTo(
            request.Vertegenwoordigers
                   .Select(
                        v =>
                            Vertegenwoordiger.Create(
                                Insz.Create(v.Insz),
                                v.IsPrimair,
                                v.Roepnaam,
                                v.Rol,
                                Voornaam.Create(v.Voornaam),
                                Achternaam.Create(v.Achternaam),
                                Email.Create(v.Email),
                                TelefoonNummer.Create(v.Telefoon),
                                TelefoonNummer.Create(v.Mobiel),
                                SocialMedia.Create(v.SocialMedia)
                            )));
    }

    private static void AssertLocaties(Locatie[] locaties, RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest request)
    {
        foreach (var (locatie, i) in locaties.Select((l, i) => (l, i)))
        {
            AssertLocatie(locatie, request.Locaties[i]);
        }
    }

    private static void AssertLocatie(Locatie locatie, ToeTeVoegenLocatie requestLocatie)
    {
        locatie.Locatietype.Waarde.Should().Be(requestLocatie.Locatietype);
        locatie.Naam.Should().Be(requestLocatie.Naam);
        locatie.IsPrimair.Should().Be(requestLocatie.IsPrimair);
        locatie.Adres!.Straatnaam.Should().Be(requestLocatie.Adres!.Straatnaam);
        locatie.Adres!.Huisnummer.Should().Be(requestLocatie.Adres!.Huisnummer);
        locatie.Adres!.Busnummer.Should().Be(requestLocatie.Adres!.Busnummer);
        locatie.Adres!.Postcode.Should().Be(requestLocatie.Adres!.Postcode);
        locatie.Adres!.Gemeente.Naam.Should().Be(requestLocatie.Adres!.Gemeente);
        locatie.Adres!.Land.Should().Be(requestLocatie.Adres!.Land);
    }

    private static void AssertContactgegevens(Contactgegeven[] contactgegevens, RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest request)
    {
        contactgegevens[0].Should().BeEquivalentTo(
            Contactgegeven.CreateFromInitiator(
                Contactgegeventype.Parse(request.Contactgegevens[0].Contactgegeventype),
                request.Contactgegevens[0].Waarde,
                request.Contactgegevens[0].Beschrijving,
                request.Contactgegevens[0].IsPrimair));
    }
}
