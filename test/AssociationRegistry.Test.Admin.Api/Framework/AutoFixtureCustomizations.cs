namespace AssociationRegistry.Test.Admin.Api.Framework;

using System.Collections.Immutable;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Projections.Historiek.Schema;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VoegContactGegevenToe;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using VCodes;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using ContactGegevens;
using ContactGegevens.Emails;
using ContactGegevens.SocialMedias;
using ContactGegevens.TelefoonNummers;
using ContactGegevens.Websites;
using Events;
using Hoofdactiviteiten;
using INSZ;
using KboNummers;
using Marten.Events;
using NodaTime;
using Primitives;
using Startdatums;
using Vereniging.RegistreerVereniging;

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
        fixture.CustomizeHoofdactiviteitVerenigingsloket();
        fixture.CustomizeHoofdactiviteitVerenigingsloketLijst();

        fixture.CustomizeRegistreerVerenigingRequest();
        fixture.CustomizeVoegContactgegevenToeRequest();

        fixture.CustomizeRegistreerVerenigingCommand();

        fixture.CustomizeVerenigingWerdGeregistreerd();
        fixture.CustomizeContactgegevenWerdToegevoegd();

        fixture.Customizations.Add(new ImmutableArraySpecimenBuilder());
        fixture.Customizations.Add(new TestEventSpecimenBuilder());

        fixture.Customizations.Add(
            new TypeRelay(
                typeof(IHistoriekData),
                typeof(HistoriekDataStub)));

        return fixture;
    }

    public static Contactgegeven CreateContactgegevenVolgensType(this Fixture source, string type)
        => type switch
        {
            nameof(ContactgegevenType.Telefoon) => source.Create<TelefoonNummer>(),
            nameof(ContactgegevenType.SocialMedia) => source.Create<SocialMedia>(),
            nameof(ContactgegevenType.Email) => source.Create<Email>(),
            nameof(ContactgegevenType.Website) => source.Create<Website>(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };

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

    public static void CustomizeRegistreerVerenigingRequest(this IFixture fixture)
    {
        fixture.Customize<RegistreerVerenigingRequest.Locatie>(
            composer => composer.FromFactory<int>(
                value => new RegistreerVerenigingRequest.Locatie
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

        fixture.Customize<RegistreerVerenigingRequest.Contactgegeven>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () =>
                    {
                        var contactgegeven = fixture.Create<Contactgegeven>();
                        return new RegistreerVerenigingRequest.Contactgegeven
                        {
                            Type = contactgegeven.Type,
                            Waarde = contactgegeven.Waarde,
                            Omschrijving = fixture.Create<string>(),
                            IsPrimair = false,
                        };
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
                        Busnummer: fixture.Create<string?>(),
                        Postcode: (fixture.Create<int>() % 10000).ToString(),
                        Gemeente: fixture.Create<string>(),
                        Land: fixture.Create<string>(),
                        Hoofdlocatie: false))
                .OmitAutoProperties());

        fixture.Customize<VerenigingWerdGeregistreerd.Contactgegeven>(
            composer => composer.FromFactory<int>(
                i =>
                {
                    var contactgegeven = fixture.Create<Contactgegeven>();
                    return new VerenigingWerdGeregistreerd.Contactgegeven(
                        i,
                        contactgegeven.Type,
                        contactgegeven.Waarde,
                        contactgegeven.Omschrijving,
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
                            contactgegeven.Omschrijving,
                            contactgegeven.IsPrimair);
                    })
                .OmitAutoProperties());
    }

    public static void CustomizeKboNummer(this IFixture fixture)
    {
        var validKboNummers = new[]
        {
            "0000000097",
            "1111111145",
            "1234.123.179",
            "1234 123 179",
            "0000 000.097",
            "1111.111 145",
            "123.1564.260",
            "12.34.56.78.94",
            ".0123456749",
            "0123456749.",
        };

        fixture.Customize<KboNummer>(
            composerTransformation: composer => composer.FromFactory<int>(
                    factory: value => KboNummer.Create(validKboNummers[value % validKboNummers.Length])!)
                .OmitAutoProperties());
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

    public static void CustomizeHoofdactiviteitVerenigingsloketLijst(this IFixture fixture)
    {
        fixture.Customize<HoofdactiviteitenVerenigingsloketLijst>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => HoofdactiviteitenVerenigingsloketLijst.Create(
                        fixture.CreateMany<HoofdactiviteitVerenigingsloket>()
                            .Distinct()
                            .ToArray()))
                .OmitAutoProperties());
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
                        _ => throw new ArgumentOutOfRangeException(),
                    })
                .OmitAutoProperties());
    }

    public static void CustomizeRegistreerVerenigingCommand(this IFixture fixture)
    {
        fixture.Customize<RegistreerVerenigingCommand>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new RegistreerVerenigingCommand(
                        Naam: fixture.Create<string>(),
                        KorteNaam: fixture.Create<string>(),
                        KorteBeschrijving: fixture.Create<string>(),
                        Startdatum: fixture.Create<Startdatum>(),
                        KboNumber: fixture.Create<KboNummer>(),
                        Contactgegevens: fixture.Create<RegistreerVerenigingCommand.Contactgegeven[]>(),
                        Locaties: fixture.Create<RegistreerVerenigingCommand.Locatie[]>(),
                        Vertegenwoordigers: fixture.Create<RegistreerVerenigingCommand.Vertegenwoordiger[]>(),
                        HoofdactiviteitenVerenigingsloket: fixture.Create<HoofdactiviteitenVerenigingsloketLijst>()
                            .Select(x => x.Code)
                            .ToArray(),
                        SkipDuplicateDetection: true)
                )
                .OmitAutoProperties());

        fixture.Customize<RegistreerVerenigingCommand.Vertegenwoordiger>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new RegistreerVerenigingCommand.Vertegenwoordiger(
                        fixture.Create<Insz>(),
                        false,
                        fixture.Create<string>(),
                        fixture.Create<string>(),
                        fixture.CreateMany<RegistreerVerenigingCommand.Contactgegeven>().ToArray()
                    ))
                .OmitAutoProperties());

        fixture.Customize<RegistreerVerenigingCommand.Contactgegeven>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () =>
                    {
                        var contactgegeven = fixture.Create<Contactgegeven>();
                        return new RegistreerVerenigingCommand.Contactgegeven(
                            contactgegeven.Type,
                            contactgegeven.Waarde,
                            contactgegeven.Omschrijving,
                            contactgegeven.IsPrimair
                        );
                    })
                .OmitAutoProperties());
    }

    public static void CustomizeVoegContactgegevenToeRequest(this IFixture fixture)
    {
        fixture.Customize<VoegContactgegevenToeRequest>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new VoegContactgegevenToeRequest
                    {
                        Contactgegeven = fixture.Create<VoegContactgegevenToeRequest.RequestContactgegeven>(),
                        Initiator = fixture.Create<VCode>(),
                    }
                )
                .OmitAutoProperties());

        fixture.Customize<VoegContactgegevenToeRequest.RequestContactgegeven>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () =>
                    {
                        var contactgegeven = fixture.Create<Contactgegeven>();
                        return new VoegContactgegevenToeRequest.RequestContactgegeven
                        {
                            Type = contactgegeven.Type,
                            Waarde = contactgegeven.Waarde,
                            Omschrijving = fixture.Create<string>(),
                            IsPrimair = false,
                        };
                    })
                .OmitAutoProperties());
    }

    public record HistoriekDataStub : IHistoriekData;
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
