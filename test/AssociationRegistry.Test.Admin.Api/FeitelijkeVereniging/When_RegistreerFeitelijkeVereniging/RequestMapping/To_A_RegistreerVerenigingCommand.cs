namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using Framework;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class To_A_RegistreerFeitelijkeVerenigingCommand
{
    [Fact]
    public void Then_We_Get_A_RegistreerFeitelijkeVerenigingCommand()
    {
        var fixture = new Fixture().CustomizeAll();

        var request = fixture.Create<RegistreerFeitelijkeVerenigingRequest>();

        var actual = request.ToCommand();

        actual.Deconstruct(
            out var naam,
            out var korteNaam,
            out var korteBeschrijving,
            out var startdatum,
            out var isUitgeschrevenUitPubliekeDatastroom,
            out var contactgegevens,
            out var locaties,
            out var vertegenwoordigers,
            out var hoofdactiviteiten,
            out var skipDuplicateDetection);

        naam.ToString().Should().Be(request.Naam);
        korteNaam.Should().Be(request.KorteNaam);
        korteBeschrijving.Should().Be(request.KorteBeschrijving);
        ((DateOnly?)startdatum).Should().Be(request.Startdatum);
        isUitgeschrevenUitPubliekeDatastroom.Should().Be(request.IsUitgeschrevenUitPubliekeDatastroom);

        AssertContactgegevens(contactgegevens, request);
        AssertLocaties(locaties, request);
        AssertVertegenwoordigers(vertegenwoordigers, request);

        hoofdactiviteiten.Select(x => x.Code).Should().BeEquivalentTo(request.HoofdactiviteitenVerenigingsloket);
        skipDuplicateDetection.Should().BeFalse();
    }

    private static void AssertVertegenwoordigers(Vertegenwoordiger[] vertegenwoordigers, RegistreerFeitelijkeVerenigingRequest request)
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

    private static void AssertLocaties(Locatie[] locaties, RegistreerFeitelijkeVerenigingRequest request)
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
        locatie.Adres!.Gemeente.Should().Be(requestLocatie.Adres!.Gemeente);
        locatie.Adres!.Land.Should().Be(requestLocatie.Adres!.Land);
        locatie.AdresId!.Adresbron.Code.Should().Be(requestLocatie.AdresId!.Broncode);
        locatie.AdresId!.Bronwaarde.Should().Be(requestLocatie.AdresId!.Bronwaarde);
    }

    private static void AssertContactgegevens(Contactgegeven[] contactgegevens, RegistreerFeitelijkeVerenigingRequest request)
    {
        contactgegevens[0].Should().BeEquivalentTo(
            Contactgegeven.Create(
                ContactgegevenType.Parse(request.Contactgegevens[0].Type),
                request.Contactgegevens[0].Waarde,
                request.Contactgegevens[0].Beschrijving,
                request.Contactgegevens[0].IsPrimair));
    }
}
