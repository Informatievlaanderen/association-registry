namespace AssociationRegistry.Test.Framework;

using AutoFixture;
using AutoFixture.Dsl;
using Events;
using NodaTime;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;
using Vereniging.Websites;

public static class AutoFixtureCustomizations
{
    public static Fixture CustomizeAll(this Fixture fixture)
    {
        fixture.CustomizeDateOnly();
        fixture.CustomizeVCode();
        fixture.CustomizeInstant();
        fixture.CustomizeInsz();
        fixture.CustomizeContactgegeven();
        fixture.CustomizeHoofdactiviteitVerenigingsloket();
        fixture.CustomizeVoornaam();
        fixture.CustomizeAchternaam();
        fixture.CustomizeLocatie();
        fixture.CustomizeKboNummer();

        fixture.CustomizeVerenigingWerdGeregistreerd();
        return fixture;
    }

    public static void CustomizeLocatie(this IFixture fixture)
    {
        fixture.Customize<Locatietype>(
            composer =>
                composer.FromFactory<int>(_ => Locatietype.Activiteiten)
                    .OmitAutoProperties()
        );

        fixture.Customize<Locatie>(
            composer => composer.FromFactory(
                () =>
                    Locatie.Create(
                        fixture.Create<string>(),
                        false,
                        Locatietype.Activiteiten,
                        fixture.Create<AdresId>(),
                        fixture.Create<Adres>())
            ).OmitAutoProperties()
        );
    }

    private static void CustomizeAchternaam(this IFixture fixture)
    {
        fixture.Customize<Achternaam>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => Achternaam.Create(string.Join("", fixture.Create<string>().Where(x => !char.IsNumber(x)))))
                .OmitAutoProperties());
    }

    private static void CustomizeVoornaam(this IFixture fixture)
    {
        fixture.Customize<Voornaam>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => Voornaam.Create(string.Join("", fixture.Create<string>().Where(x => !char.IsNumber(x)))))
                .OmitAutoProperties());
    }

    public static void CustomizeDateOnly(this IFixture fixture)
    {
        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));
    }

    public static void CustomizeVCode(this IFixture fixture)
    {
        fixture.Customize<VCode>(
            customization => customization.FromFactory(
                generator => VCode.Create(generator.Next(10000, 100000))));
    }

    public static IPostprocessComposer<T> FromFactory<T>(this IFactoryComposer<T> composer, Func<Random, T> factory)
    {
        return composer.FromFactory<int>(value => factory(new Random(value)));
    }

    public static void CustomizeInstant(this IFixture fixture)
    {
        fixture.Customize<Instant>(
            composer => composer.FromFactory(
                generator => new Instant() + Duration.FromSeconds(generator.Next())));
    }

    public static void CustomizeInsz(this IFixture fixture)
    {
        fixture.Customize<Insz>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () =>
                    {
                        var inszBase = new Random().Next(0, 999999999);
                        var inszModulo = 97 - (inszBase % 97);
                        return Insz.Create($"{inszBase:D9}{inszModulo:D2}");
                    })
                .OmitAutoProperties()
        );
    }

    public static void CustomizeContactgegeven(this IFixture fixture)
    {
        fixture.Customize<Email>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new Email($"{fixture.Create<string>()}@example.org", fixture.Create<string>(), false))
                .OmitAutoProperties());

        fixture.Customize<SocialMedia>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new SocialMedia($"https://{fixture.Create<string>()}.com", fixture.Create<string>(), false))
                .OmitAutoProperties());

        fixture.Customize<Website>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new Website($"https://{fixture.Create<string>()}.com", fixture.Create<string>(), false))
                .OmitAutoProperties());
        fixture.Customize<TelefoonNummer>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new TelefoonNummer(fixture.Create<int>().ToString(), fixture.Create<string>(), false))
                .OmitAutoProperties());

        fixture.Customize<ContactgegevenType>(
            composerTransformation: composer => composer.FromFactory<int>(
                factory: value =>
                {
                    var contactTypes = ContactgegevenType.All;
                    return contactTypes[value % contactTypes.Length];
                }).OmitAutoProperties());

        fixture.Customize<Contactgegeven>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => (string)fixture.Create<ContactgegevenType>() switch
                    {
                        nameof(ContactgegevenType.Email) => fixture.Create<Email>(),
                        nameof(ContactgegevenType.Website) => fixture.Create<Website>(),
                        nameof(ContactgegevenType.SocialMedia) => fixture.Create<SocialMedia>(),
                        nameof(ContactgegevenType.Telefoon) => fixture.Create<TelefoonNummer>(),
                        _ => throw new ArgumentOutOfRangeException($"I'm sorry Dave, I don't know how to create a Contactgegeven of this type."),
                    })
                .OmitAutoProperties());
    }

    public static void CustomizeKboNummer(this IFixture fixture)
    {
        fixture.Customize<KboNummer>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () =>
                    {
                        var kboBase = new Random().Next(0, 99999999);
                        var kboModulo = 97 - (kboBase % 97);
                        return KboNummer.Create($"{kboBase:D8}{kboModulo:D2}");
                    })
                .OmitAutoProperties()
        );
    }

    public static void CustomizeVerenigingWerdGeregistreerd(this IFixture fixture)
    {

        fixture.Customize<Adresbron>(
            composer =>
                composer.FromFactory<int>(i => Adresbron.All[i % Adresbron.All.Length])
                    .OmitAutoProperties()
        );

        fixture.Customize<AdresId>(
            composer =>
                composer.FromFactory<int>(i => AdresId.Create(
                        fixture.Create<Adresbron>(),
                        AdresId.DataVlaanderenAdresPrefix + i))
                    .OmitAutoProperties()
        );

        fixture.Customize<Registratiedata.AdresId>(
            composer =>
                composer.FromFactory<int>(_ => Registratiedata.AdresId.With(
                        fixture.Create<AdresId>())!)
                    .OmitAutoProperties()
        );

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

        fixture.Customize<Registratiedata.HoofdactiviteitVerenigingsloket>(
            composer => composer.FromFactory(
                () =>
                {
                    var h = fixture.Create<HoofdactiviteitVerenigingsloket>();
                    return new Registratiedata.HoofdactiviteitVerenigingsloket(h.Code, h.Beschrijving);
                }).OmitAutoProperties());

        fixture.Customize<Registratiedata.Contactgegeven>(
            composer => composer.FromFactory<int>(
                i =>
                {
                    var contactgegeven = fixture.Create<Contactgegeven>();
                    return new Registratiedata.Contactgegeven(
                        i,
                        contactgegeven.Type,
                        contactgegeven.Waarde,
                        contactgegeven.Beschrijving,
                        contactgegeven.IsPrimair
                    );
                }).OmitAutoProperties());

        fixture.Customize<FeitelijkeVerenigingWerdGeregistreerd>(
            composer => composer.FromFactory(
                () => new FeitelijkeVerenigingWerdGeregistreerd(
                    fixture.Create<VCode>().ToString(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<DateOnly?>(),
                    false,
                    fixture.CreateMany<Registratiedata.Contactgegeven>().ToArray(),
                    fixture.CreateMany<Registratiedata.Locatie>().ToArray(),
                    fixture.CreateMany<Registratiedata.Vertegenwoordiger>().ToArray(),
                    fixture.CreateMany<Registratiedata.HoofdactiviteitVerenigingsloket>().ToArray()
                )).OmitAutoProperties());

        fixture.Customize<AfdelingWerdGeregistreerd>(
            composer => composer.FromFactory(
                () => new AfdelingWerdGeregistreerd(
                    fixture.Create<VCode>().ToString(),
                    fixture.Create<string>(),
                    new AfdelingWerdGeregistreerd.MoederverenigingsData(
                        fixture.Create<KboNummer>(),
                        fixture.Create<VCode>(),
                        fixture.Create<VerenigingsNaam>()),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<DateOnly?>(),
                    fixture.CreateMany<Registratiedata.Contactgegeven>().ToArray(),
                    fixture.CreateMany<Registratiedata.Locatie>().ToArray(),
                    fixture.CreateMany<Registratiedata.Vertegenwoordiger>().ToArray(),
                    fixture.CreateMany<Registratiedata.HoofdactiviteitVerenigingsloket>().ToArray()
                )).OmitAutoProperties());
    }

    public static void CustomizeHoofdactiviteitVerenigingsloket(this IFixture fixture)
    {
        fixture.Customize<HoofdactiviteitVerenigingsloket>(
            composerTransformation: composer => composer.FromFactory<int>(
                    factory: value => HoofdactiviteitVerenigingsloket.All()[value % HoofdactiviteitVerenigingsloket.All().Count])
                .OmitAutoProperties());
    }
}
