namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestMapping;

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
        locaties.Should().BeEquivalentTo(request.Locaties, options => options.Excluding(x => x.AdresId));
        foreach (var (locatie, i) in locaties.Select((l, i) => (l, i)))
        {
            locatie.AdresId!.Bronwaarde.Should().Be(request.Locaties[i].AdresId!.Bronwaarde);
            locatie.AdresId!.Adresbron.Should().BeEquivalentTo(Adresbron.Parse(request.Locaties[i].AdresId!.Broncode));
        }
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
