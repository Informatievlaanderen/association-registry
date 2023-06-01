namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
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
public class To_A_RegistreerAfdelingCommand
{
    [Fact]
    public void Then_We_Get_A_RegistreerAfdelingCommand()
    {
        var fixture = new Fixture().CustomizeAll();

        var request = fixture.Create<RegistreerAfdelingRequest>();

        var actual = request.ToCommand();

        actual.Deconstruct(
            out var naam,
            out var kboNummerMoedervereniging,
            out var korteNaam,
            out var korteBeschrijving,
            out var startdatum,
            out var contactgegevens,
            out var locaties,
            out var vertegenwoordigers,
            out var hoofdactiviteiten,
            out var skipDuplicateDetection);

        naam.ToString().Should().Be(request.Naam);
        kboNummerMoedervereniging.ToString().Should().Be(request.KboNummerMoedervereniging);
        korteNaam.Should().Be(request.KorteNaam);
        korteBeschrijving.Should().Be(request.KorteBeschrijving);
        ((DateOnly?)startdatum).Should().Be(request.Startdatum);
        contactgegevens[0].Should().BeEquivalentTo(
            Contactgegeven.Create(
                ContactgegevenType.Parse(request.Contactgegevens[0].Type),
                request.Contactgegevens[0].Waarde,
                request.Contactgegevens[0].Beschrijving,
                request.Contactgegevens[0].IsPrimair));
        locaties.Should().BeEquivalentTo(request.Locaties);
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

        hoofdactiviteiten.Select(x => x.Code).Should().BeEquivalentTo(request.HoofdactiviteitenVerenigingsloket);
        skipDuplicateDetection.Should().BeFalse();
    }
}
