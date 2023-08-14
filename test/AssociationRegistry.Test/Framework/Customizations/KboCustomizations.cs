namespace AssociationRegistry.Test.Framework.Customizations;

using AutoFixture;
using Kbo;
using Vereniging;
using Vereniging.Emails;
using Vereniging.TelefoonNummers;
using Vereniging.Websites;

public static class KboCustomizations
{
    public static void CustomizeFromKbo(Fixture fixture)
    {
        fixture.CustomizeAdresVolgensKbo();
        fixture.CustomizeContactgegevensVolgensKbo();
    }

    private static void CustomizeAdresVolgensKbo(this Fixture fixture)
    {
        fixture.Customize<AdresVolgensKbo>(
            composer =>
                composer.FromFactory(
                    () =>
                    {
                        var adres = fixture.Create<Adres>();
                        return new AdresVolgensKbo
                        {
                            Straatnaam = adres.Straatnaam,
                            Huisnummer = adres.Huisnummer,
                            Busnummer = adres.Busnummer,
                            Postcode = adres.Postcode,
                            Gemeente = adres.Gemeente,
                            Land = adres.Land,
                        };
                    }));
    }

    private static void CustomizeContactgegevensVolgensKbo(this Fixture fixture)
    {
        fixture.Customize<ContactgegevensVolgensKbo>(
            composer =>
                composer.FromFactory(
                    () => new ContactgegevensVolgensKbo
                    {
                        Email = fixture.Create<Email>().Waarde,
                        Website = fixture.Create<Website>().Waarde,
                        Telefoonnummer = fixture.Create<TelefoonNummer>().Waarde,
                    }));
    }
}
