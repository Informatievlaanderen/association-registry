namespace AssociationRegistry.Test.Locaties.When_VoegLocatieToe;

using Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using DecentraalBeheer.Vereniging.Exceptions;
using Xunit;

public class Given_A_MaatschappelijkeZetel
{
    [Fact]
    public void Then_it_throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingOfAnyKind();
        var locatie = fixture.Create<Registratiedata.Locatie>();

        Assert.Throws<MaatschappelijkeZetelIsNietToegestaan>(() => vereniging.VoegLocatieToe(
                                                                 Locatie.Create(
                                                                     Locatienaam.Create(locatie.Naam),
                                                                     locatie.IsPrimair,
                                                                     Locatietype.MaatschappelijkeZetelVolgensKbo,
                                                                     AdresId.Create(locatie.AdresId!.Broncode, locatie.AdresId.Bronwaarde),
                                                                     Adres.Create(locatie.Adres!.Straatnaam,
                                                                                  locatie.Adres.Huisnummer,
                                                                                  locatie.Adres.Busnummer,
                                                                                  locatie.Adres.Postcode,
                                                                                  locatie.Adres.Gemeente,
                                                                                  locatie.Adres.Land))));
    }
}
