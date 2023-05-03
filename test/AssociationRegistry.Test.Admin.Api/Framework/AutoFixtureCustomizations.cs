namespace AssociationRegistry.Test.Admin.Api.Framework;

using System.Collections.Immutable;
using Acties.RegistreerVereniging;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VoegContactGegevenToe;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using Events;
using Marten.Events;
using NodaTime;
using Primitives;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;
using Vereniging.Websites;
using Contactgegeven = Vereniging.Contactgegeven;
using ToeTeVoegenContactgegeven = AssociationRegistry.Admin.Api.Verenigingen.Common.ToeTeVoegenContactgegeven;

public static class AutoFixtureCustomizations
{
    public static Fixture CustomizeAll(this Fixture fixture)
    {
        fixture.CustomizeDateOnly();
        fixture.CustomizeVCode();
        fixture.CustomizeInstant();
        fixture.CustomizeKboNummer();
        fixture.CustomizeInsz();
        fixture.CustomizeContactgegeven();
        fixture.CustomizeVertegenwoordiger();
        fixture.CustomizeContactgegevens();
        fixture.CustomizeHoofdactiviteitVerenigingsloket();
        fixture.CustomizeHoofdactiviteitenVerenigingsloket();

        fixture.CustomizeRegistreerVerenigingRequest();
        fixture.CustominzeWijzigBasisgegevensRequest();
        fixture.CustomizeVoegContactgegevenToeRequest();

        fixture.CustomizeRegistreerVerenigingCommand();

        fixture.CustomizeVerenigingWerdGeregistreerd();
        fixture.CustomizeContactgegevenWerdToegevoegd();

        fixture.Customizations.Add(new ImmutableArraySpecimenBuilder());
        fixture.Customizations.Add(new TestEventSpecimenBuilder());

        return fixture;
    }

    public static Contactgegeven CreateContactgegevenVolgensType(this Fixture source, string type)
    {
        string waarde = type switch
        {
            { } c when c == ContactgegevenType.Email => source.Create<Email>().ToString(),
            { } c when c == ContactgegevenType.Website => source.Create<Website>().ToString(),
            { } c when c == ContactgegevenType.SocialMedia => source.Create<SocialMedia>().ToString(),
            { } c when c == ContactgegevenType.Telefoon => source.Create<TelefoonNummer>().ToString(),
            _ => throw new ArgumentOutOfRangeException($"I'm sorry Dave, I don't know how to create a Contactgegeven of this type."),
        };
        return Contactgegeven.Create(type, waarde, source.Create<string>(), false);
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

    public static void CustominzeWijzigBasisgegevensRequest(this IFixture fixture)
    {
        fixture.Customize<WijzigBasisgegevensRequest>(
            composer => composer.FromFactory(
                () => new WijzigBasisgegevensRequest
                {
                    Naam = fixture.Create<string>(),
                    KorteNaam = fixture.Create<string>(),
                    KorteBeschrijving = fixture.Create<string>(),
                    Initiator = fixture.Create<string>(),
                    Startdatum = NullOrEmpty<DateOnly>.Create(fixture.Create<DateOnly>()),
                    HoofdactiviteitenVerenigingsloket = fixture
                        .CreateMany<HoofdactiviteitVerenigingsloket>()
                        .Distinct()
                        .Select(h => h.Code)
                        .ToArray(),
                }).OmitAutoProperties());
    }

    public static void CustomizeRegistreerVerenigingRequest(this IFixture fixture)
    {
        fixture.Customize<RegistreerVerenigingRequest>(
            composer => composer.FromFactory<int>(
                _ => new RegistreerVerenigingRequest
                {
                    Contactgegevens = fixture.CreateMany<ToeTeVoegenContactgegeven>().ToArray(),
                    Initiator = fixture.Create<string>(),
                    Locaties = fixture.CreateMany<ToeTeVoegenLocatie>().ToArray(),
                    Startdatum = fixture.Create<Startdatum>(),
                    Naam = fixture.Create<string>(),
                    Vertegenwoordigers = fixture.CreateMany<ToeTeVoegenVertegenwoordiger>().ToArray(),
                    HoofdactiviteitenVerenigingsloket = fixture.CreateMany<HoofdactiviteitVerenigingsloket>()
                        .Select(x => x.Code)
                        .ToArray(),
                    KorteBeschrijving = fixture.Create<string>(),
                    KorteNaam = fixture.Create<string>(),
                    KboNummer = fixture.Create<KboNummer>(),
                }).OmitAutoProperties());

        fixture.Customize<ToeTeVoegenLocatie>(
            composer => composer.FromFactory<int>(
                value => new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietypes.All[value % Locatietypes.All.Length],
                    Naam = fixture.Create<string>(),
                    Straatnaam = fixture.Create<string>(),
                    Huisnummer = fixture.Create<int>().ToString(),
                    Busnummer = fixture.Create<string?>(),
                    Postcode = (fixture.Create<int>() % 10000).ToString(),
                    Gemeente = fixture.Create<string>(),
                    Land = fixture.Create<string>(),
                    Hoofdlocatie = false,
                }).OmitAutoProperties());

        fixture.Customize<ToeTeVoegenContactgegeven>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () =>
                    {
                        var contactgegeven = fixture.Create<Contactgegeven>();
                        return new ToeTeVoegenContactgegeven
                        {
                            Type = contactgegeven.Type,
                            Waarde = contactgegeven.Waarde,
                            Beschrijving = fixture.Create<string>(),
                            IsPrimair = false,
                        };
                    })
                .OmitAutoProperties());

        fixture.Customize<ToeTeVoegenVertegenwoordiger>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new ToeTeVoegenVertegenwoordiger
                    {
                        Insz = fixture.Create<Insz>(),
                        Roepnaam = fixture.Create<string>(),
                        Rol = fixture.Create<string>(),
                        IsPrimair = false,
                        Email = fixture.Create<Email>().Waarde,
                        Telefoon = fixture.Create<TelefoonNummer>().Waarde,
                        Mobiel = fixture.Create<TelefoonNummer>().Waarde,
                        SocialMedia = fixture.Create<SocialMedia>().Waarde,
                    })
                .OmitAutoProperties());
    }

    public static void CustomizeVerenigingWerdGeregistreerd(this IFixture fixture)
    {
        fixture.Customize<VerenigingWerdGeregistreerd.Locatie>(
            composer => composer.FromFactory<int>(
                    value => new VerenigingWerdGeregistreerd.Locatie(
                        Locatietype: Locatietypes.All[value % Locatietypes.All.Length],
                        Naam: fixture.Create<string>(),
                        Straatnaam: fixture.Create<string>(),
                        Huisnummer: fixture.Create<int>().ToString(),
                        Busnummer: fixture.Create<string>(),
                        Postcode: (fixture.Create<int>() % 10000).ToString(),
                        Gemeente: fixture.Create<string>(),
                        Land: fixture.Create<string>(),
                        Hoofdlocatie: false))
                .OmitAutoProperties());

        fixture.Customize<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>(
            composer => composer.FromFactory(
                () =>
                {
                    var h = fixture.Create<HoofdactiviteitVerenigingsloket>();
                    return new VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket(h.Code, h.Beschrijving);
                }).OmitAutoProperties());

        fixture.Customize<VerenigingWerdGeregistreerd.Contactgegeven>(
            composer => composer.FromFactory<int>(
                i =>
                {
                    var contactgegeven = fixture.Create<Contactgegeven>();
                    return new VerenigingWerdGeregistreerd.Contactgegeven(
                        i,
                        contactgegeven.Type,
                        contactgegeven.Waarde,
                        contactgegeven.Beschrijving,
                        contactgegeven.IsPrimair
                    );
                }).OmitAutoProperties());
    }

    public static void CustomizeContactgegevenWerdToegevoegd(this IFixture fixture)
    {
        fixture.Customize<ContactgegevenWerdToegevoegd>(
            composer => composer.FromFactory(
                    () =>
                    {
                        var contactgegeven = fixture.Create<Contactgegeven>();
                        return new ContactgegevenWerdToegevoegd(
                            contactgegeven.ContactgegevenId,
                            contactgegeven.Type,
                            contactgegeven.Waarde,
                            contactgegeven.Beschrijving,
                            contactgegeven.IsPrimair);
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

    public static void CustomizeHoofdactiviteitVerenigingsloket(this IFixture fixture)
    {
        fixture.Customize<HoofdactiviteitVerenigingsloket>(
            composerTransformation: composer => composer.FromFactory<int>(
                    factory: value => HoofdactiviteitVerenigingsloket.All()[value % HoofdactiviteitVerenigingsloket.All().Count])
                .OmitAutoProperties());
    }

    public static void CustomizeHoofdactiviteitenVerenigingsloket(this IFixture fixture)
    {
        fixture.Customize<HoofdactiviteitenVerenigingsloket>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => HoofdactiviteitenVerenigingsloket.FromArray(
                        fixture.CreateMany<HoofdactiviteitVerenigingsloket>()
                            .Distinct()
                            .ToArray()))
                .OmitAutoProperties());
    }

    public static void CustomizeVertegenwoordiger(this IFixture fixture)
    {
        fixture.Customize<Vertegenwoordiger>(
            composer => composer.FromFactory(
                () => Vertegenwoordiger.Create(
                    fixture.Create<Insz>(),
                    false,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<Email>(),
                    fixture.Create<TelefoonNummer>(),
                    fixture.Create<TelefoonNummer>(),
                    fixture.Create<SocialMedia>()
                )).OmitAutoProperties());
    }

    public static void CustomizeContactgegevens(this IFixture fixture)
    {
        fixture.Customize<Contactgegevens>(
            composer => composer.FromFactory(
                () => Contactgegevens.FromArray(
                    fixture.CreateMany<Contactgegeven>().ToArray())));
    }

    public static void CustomizeContactgegeven(this IFixture fixture)
    {
        fixture.Customize<Email>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new Email($"{fixture.Create<string>()}@example.org"))
                .OmitAutoProperties());

        fixture.Customize<SocialMedia>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new SocialMedia($"https://{fixture.Create<string>()}.com"))
                .OmitAutoProperties());

        fixture.Customize<Website>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new Website($"https://{fixture.Create<string>()}.com"))
                .OmitAutoProperties());
        fixture.Customize<TelefoonNummer>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new TelefoonNummer(fixture.Create<int>().ToString()))
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
                    factory: () =>
                    {
                        var contactgegevenType = fixture.Create<ContactgegevenType>();
                        var waarde = (string)contactgegevenType switch
                        {
                            { } c when c == ContactgegevenType.Email => fixture.Create<Email>().ToString(),
                            { } c when c == ContactgegevenType.Website => fixture.Create<Website>().ToString(),
                            { } c when c == ContactgegevenType.SocialMedia => fixture.Create<SocialMedia>().ToString(),
                            { } c when c == ContactgegevenType.Telefoon => fixture.Create<TelefoonNummer>().ToString(),
                            _ => throw new ArgumentOutOfRangeException($"I'm sorry Dave, I don't know how to create a Contactgegeven of this type."),
                        };
                        return Contactgegeven.Create(contactgegevenType, waarde, fixture.Create<string>(), false);
                    })
                .OmitAutoProperties());
    }

    public static void CustomizeRegistreerVerenigingCommand(this IFixture fixture)
    {
        fixture.Customize<RegistreerVerenigingCommand>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new RegistreerVerenigingCommand(
                        Naam: fixture.Create<VerenigingsNaam>(),
                        KorteNaam: fixture.Create<string>(),
                        KorteBeschrijving: fixture.Create<string>(),
                        Startdatum: fixture.Create<Startdatum>(),
                        KboNummer: fixture.Create<KboNummer>(),
                        Contactgegevens: fixture.CreateMany<Contactgegeven>().ToArray(),
                        Locaties: fixture.CreateMany<Locatie>().ToArray(),
                        Vertegenwoordigers: fixture.CreateMany<Vertegenwoordiger>().ToArray(),
                        HoofdactiviteitenVerenigingsloket: fixture.CreateMany<HoofdactiviteitVerenigingsloket>().Distinct().ToArray(),
                        SkipDuplicateDetection: true)
                )
                .OmitAutoProperties());
    }

    public static void CustomizeVoegContactgegevenToeRequest(this IFixture fixture)
    {
        fixture.Customize<VoegContactgegevenToeRequest>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new VoegContactgegevenToeRequest
                    {
                        Contactgegeven = fixture.Create<ToeTeVoegenContactgegeven>(),
                        Initiator = fixture.Create<VCode>(),
                    }
                )
                .OmitAutoProperties());

        fixture.Customize<ToeTeVoegenContactgegeven>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () =>
                    {
                        var contactgegeven = fixture.Create<Contactgegeven>();
                        return new ToeTeVoegenContactgegeven
                        {
                            Type = contactgegeven.Type,
                            Waarde = contactgegeven.Waarde,
                            Beschrijving = fixture.Create<string>(),
                            IsPrimair = false,
                        };
                    })
                .OmitAutoProperties());
    }
}

public class ImmutableArraySpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (request is not Type t)
            return new NoSpecimen();

        var typeArguments = t.GetGenericArguments();
        if (typeArguments.Length != 1)
            return new NoSpecimen();

        if (typeof(ImmutableArray<>) != t.GetGenericTypeDefinition())
            return new NoSpecimen();

        dynamic list = context.Resolve(typeof(IList<>).MakeGenericType(typeArguments));
        return ImmutableArray.ToImmutableArray(list);
    }
}

public class TestEventSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (request is not Type t)
            return new NoSpecimen();

        var typeArguments = t.GetGenericArguments();
        if (typeArguments.Length != 1)
            return new NoSpecimen();

        if (typeof(TestEvent<>) != t.GetGenericTypeDefinition())
            return new NoSpecimen();

        var @event = context.Resolve(typeArguments.Single());
        var instance = (IEvent)Activator.CreateInstance(t, @event, context.Create<string>(), context.Create<Instant>())!;
        instance.Version = context.Create<long>();
        instance.Sequence = context.Create<long>();
        return instance;
    }
}
