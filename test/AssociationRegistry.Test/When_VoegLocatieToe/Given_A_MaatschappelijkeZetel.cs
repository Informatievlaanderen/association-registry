namespace AssociationRegistry.Test.When_VoegLocatieToe;

using AutoFixture;
using Common.AutoFixture;
using Events;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
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
