namespace AssociationRegistry.Test.Framework.Customizations;

using Events;
using Vereniging;
using AutoFixture;

public static class RegistratiedataCustomizations
{
    public static void CustomizeRegistratiedata(IFixture fixture)
    {
        fixture.CustomizeAdresId();
        fixture.CustomizeLocatie();
        fixture.CustomizeHoofdactiviteitVerenigingsloket();
        fixture.CustomizeContactgegeven();
        fixture.CustomizeDoelgroep();
    }

    private static void CustomizeContactgegeven(this IFixture fixture)
    {
        fixture.Customize<Registratiedata.Contactgegeven>(
            composer => composer.FromFactory<int>(
                i =>
                {
                    var contactgegeven = fixture.Create<Contactgegeven>();
                    return new Registratiedata.Contactgegeven(
                        i,
                        contactgegeven.Type,
                        contactgegeven.Waarde.Waarde,
                        contactgegeven.Beschrijving,
                        contactgegeven.IsPrimair
                    );
                }).OmitAutoProperties());
    }

    private static void CustomizeHoofdactiviteitVerenigingsloket(this IFixture fixture)
    {
        fixture.Customize<Registratiedata.HoofdactiviteitVerenigingsloket>(
            composer => composer.FromFactory(
                () =>
                {
                    var h = fixture.Create<HoofdactiviteitVerenigingsloket>();
                    return new Registratiedata.HoofdactiviteitVerenigingsloket(h.Code, h.Beschrijving);
                }).OmitAutoProperties());
    }

    private static void CustomizeLocatie(this IFixture fixture)
    {
        fixture.Customize<Registratiedata.Locatie>(
            composer => composer.FromFactory(
                () => new Registratiedata.Locatie(
                    LocatieId: fixture.Create<int>(),
                    Locatietype: fixture.Create<Locatietype>(),
                    IsPrimair: false,
                    Naam: fixture.Create<string>(),
                    Adres: new Registratiedata.Adres(
                        Straatnaam: fixture.Create<string>(),
                        Huisnummer: fixture.Create<int>().ToString(),
                        Busnummer: fixture.Create<string>(),
                        Postcode: (fixture.Create<int>() % 10000).ToString(),
                        Gemeente: fixture.Create<string>(),
                        Land: fixture.Create<string>()),
                    AdresId: fixture.Create<Registratiedata.AdresId>())).OmitAutoProperties());
    }

    private static void CustomizeAdresId(this IFixture fixture)
    {
        fixture.Customize<Registratiedata.AdresId>(
            composer =>
                composer.FromFactory<int>(
                        _ => Registratiedata.AdresId.With(
                            fixture.Create<AdresId>())!)
                    .OmitAutoProperties()
        );
    }

    private static void CustomizeDoelgroep(this IFixture fixture)
    {
        fixture.Customize<Registratiedata.Doelgroep>(
            composer =>
                composer.FromFactory(
                        () => Registratiedata.Doelgroep.With(
                            fixture.Create<Doelgroep>()))
                    .OmitAutoProperties()
        );
    }
}
