namespace AssociationRegistry.Test.Common.AutoFixture;

using Admin.Schema.PowerBiExport;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using DecentraalBeheer.Vereniging.Emails;
using DecentraalBeheer.Vereniging.SocialMedias;
using DecentraalBeheer.Vereniging.TelefoonNummers;
using DecentraalBeheer.Vereniging.Websites;
using Events;
using Framework;
using global::AutoFixture;
using global::AutoFixture.Dsl;
using NodaTime;
using Vereniging;
using HoofdactiviteitVerenigingsloket = DecentraalBeheer.Vereniging.HoofdactiviteitVerenigingsloket;
using Lidmaatschap = DecentraalBeheer.Vereniging.Lidmaatschap;
using Werkingsgebied = DecentraalBeheer.Vereniging.Werkingsgebied;

public static class AutoFixtureCustomizations
{
    public static Fixture CustomizeDomain(this Fixture fixture)
    {
        fixture.CustomizeDateOnly();
        fixture.CustomizeVCode();
        fixture.CustomizeInstant();
        fixture.CustomizeInsz();
        fixture.CustomizeContactgegeven();
        fixture.CustomizeHoofdactiviteitVerenigingsloket();
        fixture.CustomizeHoofdactiviteitenVerenigingsloket();
        fixture.CustomizeVoornaam();
        fixture.CustomizeAchternaam();
        fixture.CustomizeLocatie();
        fixture.CustomizeKboNummer();
        fixture.CustomizeVerengingsType();
        fixture.CustomizeVertegenwoordiger();
        fixture.CustomizeAdresBron();
        fixture.CustomizeDoelgroep();
        fixture.CustomizeAdresId();
        fixture.CustomizeAdres();
        fixture.CustomizeGebeurtenis();
        fixture.CustomizeMagdaResponses();
        fixture.CustomizeWerkingsgebied();
        fixture.CustomizeLidmaatschap();
        fixture.CustomizeGeldigheidsperiode();

        RegistratiedataCustomizations.CustomizeRegistratiedata(fixture);
        EventCustomizations.CustomizeEvents(fixture);
        CommandCustomizations.CustomizeCommands(fixture);
        KboCustomizations.CustomizeFromKbo(fixture);

        fixture.Customizations.Add(new ImmutableArraySpecimenBuilder());
        fixture.Customizations.Add(new StartDatumCustomization());

        return fixture;
    }

    public static void CustomizeTestEvent(this Fixture fixture, Type testEventType)
    {
        fixture.Customizations.Add(new TestEventSpecimenBuilder(testEventType));
    }

    public static Contactgegeven CreateContactgegevenVolgensType(this IFixture source, string type)
        => type switch
        {
            Contactgegeventype.Labels.Telefoon => source.Create<TelefoonNummer>(),
            Contactgegeventype.Labels.SocialMedia => source.Create<SocialMedia>(),
            Contactgegeventype.Labels.Email => source.Create<Email>(),
            Contactgegeventype.Labels.Website => source.Create<Website>(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, message: null),
        };

    private static void CustomizeVertegenwoordiger(this IFixture fixture)
    {
        fixture.Customize<Vertegenwoordiger>(
            composer => composer.FromFactory(
                () => Vertegenwoordiger.Create(
                    false
                )).OmitAutoProperties());
    }

    private static void CustomizeLocatie(this IFixture fixture)
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
                        fixture.Create<Locatienaam>(),
                        isPrimair: false,
                        Locatietype.Activiteiten,
                        adresId: null,
                        fixture.Create<Adres>())
            ).OmitAutoProperties()
        );
    }

    private static void CustomizeAchternaam(this IFixture fixture)
    {
        fixture.Customize<Achternaam>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => Achternaam.Create(
                                                                 string.Join(separator: "",
                                                                             fixture.Create<string>().Where(x => !char.IsNumber(x)))))
                                                        .OmitAutoProperties());
    }

    private static void CustomizeVoornaam(this IFixture fixture)
    {
        fixture.Customize<Voornaam>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => Voornaam.Create(
                                                                 string.Join(separator: "",
                                                                             fixture.Create<string>().Where(x => !char.IsNumber(x)))))
                                                        .OmitAutoProperties());
    }

    private static void CustomizeDateOnly(this IFixture fixture)
    {
        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));
    }

    private static int vCode = 10000;
    private static void CustomizeVCode(this IFixture fixture)
    {
        fixture.Customize<VCode>(
            customization => customization.FromFactory(
                generator => VCode.Create(++vCode)));
    }


    private static IPostprocessComposer<T> FromFactory<T>(this IFactoryComposer<T> composer, Func<Random, T> factory)
    {
        return composer.FromFactory<int>(value => factory(new Random(value)));
    }

    private static void CustomizeInstant(this IFixture fixture)
    {
        fixture.Customize<Instant>(
            composer => composer.FromFactory(
                generator => new Instant() + Duration.FromSeconds(generator.Next())));
    }

    private static void CustomizeInsz(this IFixture fixture)
    {
        fixture.Customize<Insz>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () =>
                                                             {
                                                                 var inszBase = new Random().Next(minValue: 0, maxValue: 999999999);
                                                                 var inszModulo = 97 - inszBase % 97;

                                                                 return Insz.Create($"{inszBase:D9}{inszModulo:D2}");
                                                             })
                                                        .OmitAutoProperties()
        );
    }

    private static void CustomizeWerkingsgebied(this IFixture fixture)
    {
        fixture.Customize<Werkingsgebied>(
            composerTransformation: composer => composer.FromFactory<int>(
                factory: value =>
                {
                    var werkingsgebieden = WerkingsgebiedenServiceMock.All;

                    return werkingsgebieden[value % werkingsgebieden.Length];
                }).OmitAutoProperties());
    }

    private static void CustomizeContactgegeven(this IFixture fixture)
    {
        fixture.Customize<Email>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => new Email(
                                                                 $"{fixture.Create<string>()}@example.org", fixture.Create<string>(),
                                                                 IsPrimair: false))
                                                        .OmitAutoProperties());

        fixture.Customize<SocialMedia>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => new SocialMedia(
                                                                 $"https://{fixture.Create<string>()}.com", fixture.Create<string>(),
                                                                 IsPrimair: false))
                                                        .OmitAutoProperties());

        fixture.Customize<Website>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => new Website(
                                                                 $"https://{fixture.Create<string>()}.com", fixture.Create<string>(),
                                                                 IsPrimair: false))
                                                        .OmitAutoProperties());

        fixture.Customize<TelefoonNummer>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => new TelefoonNummer(
                                                                 fixture.Create<int>().ToString(), fixture.Create<string>(),
                                                                 IsPrimair: false))
                                                        .OmitAutoProperties());

        fixture.Customize<Contactgegeventype>(
            composerTransformation: composer => composer.FromFactory<int>(
                factory: value =>
                {
                    var contactTypes = Contactgegeventype.All;

                    return contactTypes[value % contactTypes.Length];
                }).OmitAutoProperties());

        fixture.Customize<ContactgegeventypeVolgensKbo>(
            composerTransformation: composer => composer.FromFactory<int>(
                factory: value =>
                {
                    var contactTypes = ContactgegeventypeVolgensKbo.All;

                    return contactTypes[value % contactTypes.Length];
                }).OmitAutoProperties());

        fixture.Customize<Contactgegeven>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => (string)fixture.Create<Contactgegeventype>() switch
                                                             {
                                                                 Contactgegeventype.Labels.Email => fixture.Create<Email>(),
                                                                 Contactgegeventype.Labels.Website => fixture.Create<Website>(),
                                                                 Contactgegeventype.Labels.SocialMedia => fixture.Create<SocialMedia>(),
                                                                 Contactgegeventype.Labels.Telefoon => fixture.Create<TelefoonNummer>(),
                                                                 _ => throw new ArgumentOutOfRangeException(
                                                                     "I'm sorry Dave, I don't know how to create a Contactgegeven of this type."),
                                                             })
                                                        .OmitAutoProperties());
    }

    private static void CustomizeKboNummer(this IFixture fixture)
    {
        fixture.Customize<KboNummer>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () =>
                                                             {
                                                                 var kboBase = new Random().Next(minValue: 0, maxValue: 99999999);
                                                                 var kboModulo = 97 - kboBase % 97;

                                                                 return KboNummer.Create($"{kboBase:D8}{kboModulo:D2}");
                                                             })
                                                        .OmitAutoProperties()
        );
    }

    private static void CustomizeVerengingsType(this IFixture fixture)
    {
        fixture.Customize<Verenigingstype>(
            composerTransformation: composer => composer.FromFactory<int>(
                                                             factory: i => Verenigingstype.All[i % Verenigingstype.All.Length])
                                                        .OmitAutoProperties()
        );
    }

    private static void CustomizeAdresBron(this IFixture fixture)
    {
        fixture.Customize<Adresbron>(
            composer =>
                composer.FromFactory<int>(i => Adresbron.All[i % Adresbron.All.Length])
                        .OmitAutoProperties()
        );
    }

    private static void CustomizeHoofdactiviteitVerenigingsloket(this IFixture fixture)
    {
        fixture.Customize<HoofdactiviteitVerenigingsloket>(
            composerTransformation: composer => composer.FromFactory<int>(
                                                             factory: value
                                                                 => HoofdactiviteitVerenigingsloket.All()[
                                                                     value % HoofdactiviteitVerenigingsloket.All().Count])
                                                        .OmitAutoProperties());
    }

    private static void CustomizeHoofdactiviteitenVerenigingsloket(this IFixture fixture)
    {
        fixture.Customize<HoofdactiviteitenVerenigingsloket>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => HoofdactiviteitenVerenigingsloket.FromArray(
                                                                 fixture.CreateMany<HoofdactiviteitVerenigingsloket>()
                                                                        .Distinct()
                                                                        .ToArray()))
                                                        .OmitAutoProperties());
    }

    private static void CustomizeDoelgroep(this IFixture fixture)
    {
        fixture.Customize<Doelgroep>(
            composer =>
                composer.FromFactory(
                             () => Doelgroep.Create(
                                 fixture.Create<int>() % 50,
                                 50 + fixture.Create<int>() % 50))
                        .OmitAutoProperties()
        );
    }

    private static void CustomizeLidmaatschap(this IFixture fixture)
    {
        fixture.Customize<Lidmaatschap>(
            composer =>
                composer.FromFactory(
                             () => Lidmaatschap.Hydrate(
                                 fixture.Create<LidmaatschapId>(),
                                 fixture.Create<VCode>(),
                                 fixture.Create<string>(),
                                 fixture.Create<Geldigheidsperiode>(),
                                 fixture.Create<string>(),
                                 fixture.Create<string>()
                             ))
                        .OmitAutoProperties()
        );
    }

    private static void CustomizeGeldigheidsperiode(this IFixture fixture)
    {
        var date = fixture.Create<DateOnly>();

        fixture.Customize<Geldigheidsperiode>(
            composer =>
                composer.FromFactory(
                             () => new Geldigheidsperiode(
                                 new GeldigVan(date),
                                 new GeldigTot(date.AddDays(new Random().Next(1, 99)))
                             ))
                        .OmitAutoProperties()
        );
    }

    private static void CustomizeAdresId(this IFixture fixture)
    {
        fixture.Customize<AdresId>(
            composer =>
                composer.FromFactory<int>(
                             i => AdresId.Create(
                                 fixture.Create<Adresbron>(),
                                 AdresId.DataVlaanderenAdresPrefix + i))
                        .OmitAutoProperties()
        );

        fixture.Customize<Admin.Api.WebApi.Verenigingen.Common.AdresId>(
            composer =>
                composer.FromFactory<int>(
                             i => new Admin.Api.WebApi.Verenigingen.Common.AdresId
                             {
                                 Bronwaarde = AdresId.DataVlaanderenAdresPrefix + i,
                                 Broncode = fixture.Create<Adresbron>(),
                             })
                        .OmitAutoProperties()
        );
    }

    private static void CustomizeAdres(this IFixture fixture)
    {
        fixture.Customize<Adres>(
            composer =>
                composer.FromFactory(
                             () => Adres.Create(
                                 fixture.Create<string>(),
                                 fixture.Create<string>(),
                                 fixture.Create<string>(),
                                 fixture.Create<string>(),
                                 fixture.Create<string>(),
                                 fixture.Create<string>()
                             ))
                        .OmitAutoProperties()
        );
    }

    private static void CustomizeGebeurtenis(this IFixture fixture)
    {
        fixture.Customize<Gebeurtenis>(
            composer =>
                composer.FromFactory(
                             () => new Gebeurtenis(
                                 fixture.Create<DateTimeOffset>().UtcDateTime.ToString(),
                                 fixture.Create<string>(),
                                 fixture.Create<string>(),
                                 fixture.Create<long>()
                             ))
                        .OmitAutoProperties()
        );
    }
}
