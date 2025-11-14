namespace AssociationRegistry.Test.Common.AutoFixture;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using DecentraalBeheer.Vereniging.Emails;
using DecentraalBeheer.Vereniging.SocialMedias;
using DecentraalBeheer.Vereniging.TelefoonNummers;
using Events;
using Events.Factories;
using global::AutoFixture;
using Vereniging;

public static class RegistratiedataCustomizations
{
    public static void CustomizeRegistratiedata(IFixture fixture)
    {
        fixture.CustomizeAdresId();
        fixture.CustomizeLocatie();
        fixture.CustomizeHoofdactiviteitVerenigingsloket();
        fixture.CustomizeWerkingsgebieden();
        fixture.CustomizeContactgegeven();
        fixture.CustomizeDoelgroep();
        fixture.CustomizeVertegenwoordiger();
    }

    private static void CustomizeVertegenwoordiger(this IFixture fixture)
    {
        fixture.Customize<Registratiedata.Vertegenwoordiger>(
            composer => composer.FromFactory(
                                     () => new Registratiedata.Vertegenwoordiger(
                                         Guid.NewGuid(),
                                         fixture.Create<int>(),
                                         IsPrimair: false
                                     ))
                                .OmitAutoProperties());
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
                        contactgegeven.Contactgegeventype,
                        contactgegeven.Waarde,
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

                    return new Registratiedata.HoofdactiviteitVerenigingsloket(h.Code, h.Naam);
                }).OmitAutoProperties());
    }

    private static void CustomizeWerkingsgebieden(this IFixture fixture)
    {
        fixture.Customize<Registratiedata.Werkingsgebied>(
            composer => composer.FromFactory(
                () =>
                {
                    var h = fixture.Create<Werkingsgebied>();

                    return new Registratiedata.Werkingsgebied(h.Code, h.Naam);
                }).OmitAutoProperties());
    }

    private static void CustomizeLocatie(this IFixture fixture)
    {
        fixture.Customize<Registratiedata.Locatie>(
            composer => composer.FromFactory(
                () => new Registratiedata.Locatie(
                    fixture.Create<int>(),
                    fixture.Create<Locatietype>(),
                    IsPrimair: false,
                    fixture.Create<string>(),
                    new Registratiedata.Adres(
                        fixture.Create<string>(),
                        fixture.Create<int>().ToString(),
                        fixture.Create<string>(),
                        (fixture.Create<int>() % 10000).ToString(),
                        fixture.Create<string>(),
                        Land: "België"),
                    fixture.Create<Registratiedata.AdresId>())).OmitAutoProperties());
    }

    private static void CustomizeAdresId(this IFixture fixture)
    {
        fixture.Customize<Registratiedata.AdresId>(
            composer =>
                composer.FromFactory<int>(
                             _ => EventFactory.AdresId(
                                 fixture.Create<AdresId>())!)
                        .OmitAutoProperties()
        );
    }

    private static void CustomizeDoelgroep(this IFixture fixture)
    {
        fixture.Customize<Registratiedata.Doelgroep>(
            composer =>
                composer.FromFactory(
                             () => EventFactory.Doelgroep(
                                 fixture.Create<Doelgroep>()))
                        .OmitAutoProperties()
        );
    }
}
