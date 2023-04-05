namespace AssociationRegistry.Test.Admin.Api.Framework;

using System.Collections.Immutable;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Projections.Historiek.Schema;
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
using Vereniging.RegistreerVereniging;

public static class AutoFixtureCustomizations
{
    public static Fixture CustomizeAll(this Fixture fixture)
    {
        fixture.CustomizeDateOnly();
        fixture.CustomizeVCode();
        fixture.CustomizeInstant();
        fixture.CustomizeRegistreerVerenigingRequestLocatie();
        fixture.CustomizeVerenigingWerdGeregistreerdLocatie();

        fixture.CustomizeKboNummer();
        fixture.CustomizeInsz();
        fixture.CustomizeRegistreerVerenigingCommandVertegenwoordiger();
        fixture.CustomizeHoofdactiviteitVerenigingsloket();
        fixture.CustomizeHoofdactiviteitVerenigingsloketLijst();
        fixture.CustomizeRegistreerVerenigingContactgegeven();
        fixture.CustomizeRegistreerVerenigingCommand();

        fixture.Customizations.Add(new ImmutableArraySpecimenBuilder());
        fixture.Customizations.Add(new TestEventSpecimenBuilder());

        fixture.Customizations.Add(
            new TypeRelay(
                typeof(IHistoriekData),
                typeof(HistoriekDataStub)));

        return fixture;
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

    public static void CustomizeRegistreerVerenigingRequestLocatie(this IFixture fixture)
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
    }

    public static void CustomizeVerenigingWerdGeregistreerdLocatie(this IFixture fixture)
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
                    factory: () => HoofdactiviteitenVerenigingsloketLijst.Create(fixture.CreateMany<HoofdactiviteitVerenigingsloket>()
                        .Distinct()
                        .ToArray()))
                .OmitAutoProperties());
    }

    public static void CustomizeRegistreerVerenigingContactgegeven(this IFixture fixture)
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
                    factory:  ()=> new Website($"https://{fixture.Create<string>()}.com", fixture.Create<string>(), false))
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

        fixture.Customize<RegistreerVerenigingCommand.Contactgegeven>(
            composerTransformation: composer => composer.FromFactory<int>(
                    factory: value =>
                    {
                        var contactTypes = ContactgegevenType.All;
                        var contactType = contactTypes[value % contactTypes.Length];
                        Contactgegeven waarde = (string)contactType switch
                        {
                            nameof(ContactgegevenType.Email) => fixture.Create<Email>(),
                            nameof(ContactgegevenType.Website) => fixture.Create<Website>(),
                            nameof(ContactgegevenType.SocialMedia) => fixture.Create<SocialMedia>(),
                            nameof(ContactgegevenType.Telefoon) => fixture.Create<TelefoonNummer>(),
                            _ => throw new ArgumentOutOfRangeException(),
                        };
                        return new RegistreerVerenigingCommand.Contactgegeven(
                            contactType,
                            waarde.Waarde,
                            fixture.Create<string>(),
                            false
                        );
                    })
                .OmitAutoProperties());
    }

    public static void CustomizeRegistreerVerenigingCommandVertegenwoordiger(this IFixture fixture)
    {
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
    }

    public static void CustomizeRegistreerVerenigingCommand(this IFixture fixture)
        {
            fixture.Customize<RegistreerVerenigingCommand>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new RegistreerVerenigingCommand(
                        Naam: fixture.Create<string>(),
                        KorteNaam: fixture.Create<string>(),
                        KorteBeschrijving: fixture.Create<string>(),
                        Startdatum: fixture.Create<NullOrEmpty<DateOnly>>(),
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
