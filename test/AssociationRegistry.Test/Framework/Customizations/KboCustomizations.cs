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
        fixture.CustomizeVerenigingVolgensKbo();
        fixture.CustomizeAdresVolgensKbo();
        fixture.CustomizeContactgegevensVolgensKbo();
    }

    private static void CustomizeVerenigingVolgensKbo(this Fixture fixture)
    {
        fixture.Customize<VerenigingVolgensKbo>(
            composer =>
                composer.FromFactory<int>(
                             i =>
                             {
                                 return new VerenigingVolgensKbo
                                 {
                                     Naam = fixture.Create<string>(),
                                     KorteNaam = fixture.Create<string>(),
                                     Adres = new AdresVolgensKbo(),
                                     Contactgegevens = new ContactgegevensVolgensKbo(),
                                     Type = new[]
                                     {
                                         Verenigingstype.IVZW, Verenigingstype.VZW, Verenigingstype.PrivateStichting,
                                         Verenigingstype.StichtingVanOpenbaarNut,
                                     }[i % 4],
                                     KboNummer = fixture.Create<KboNummer>(),
                                     Startdatum = fixture.Create<DateOnly>(),
                                     IsActief = true,
                                     EindDatum = null,
                                 };
                             })
                        .OmitAutoProperties());
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
                                     Gemeente = adres.Gemeente.Naam,
                                     Land = adres.Land,
                                 };
                             })
                        .OmitAutoProperties());
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
                        GSM = fixture.Create<TelefoonNummer>().Waarde,
                    }).OmitAutoProperties());
    }
}
