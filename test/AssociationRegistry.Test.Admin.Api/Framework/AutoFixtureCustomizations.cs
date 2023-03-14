namespace AssociationRegistry.Test.Admin.Api.Framework;

using System.Collections.Immutable;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using VCodes;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using Events;
using NodaTime;
using Vereniging.CommonCommandDataTypes;

public static class AutoFixtureCustomizations
{
    public static Fixture CustomizeAll(this Fixture fixture)
    {
        fixture.CustomizeDateOnly();
        fixture.CustomizeVCode();
        fixture.CustomizeInstant();
        fixture.CustomizeRegistreerVerenigingRequestLocatie();
        fixture.CustomizeVerenigingWerdGeregistreerdLocatie();
        fixture.CustomizeImmutableArray();
        fixture.CustomizeContactInfo();
        fixture.CustomizeRequestContactInfo();
        fixture.CustomizeEventContactInfo();
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

    public static void CustomizeImmutableArray(this IFixture fixture)
    {
        fixture.Customizations.Add(new ImmutableArraySpecimenBuilder());
    }

    public static void CustomizeContactInfo(this IFixture fixture)
    {
        fixture.Customize<ContactInfo>(
            composer => composer.FromFactory(
                    () => new ContactInfo(
                        fixture.Create<string>(),
                        $"a{fixture.Create<string>()}@example.org",
                        fixture.Create<uint>().ToString(),
                        $"https://{fixture.Create<string>()}.vlaanderen",
                        $"https://{fixture.Create<string>()}.vlaanderen",
                        false
                    )
                ).OmitAutoProperties());
    }

    public static void CustomizeRequestContactInfo(this IFixture fixture)
    {
        fixture.Customize<AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes.ContactInfo>(
            composer => composer.FromFactory(
                () => new AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes.ContactInfo
                {
                    Contactnaam = fixture.Create<string>(),
                    Email = $"a{fixture.Create<string>()}@example.org",
                    Telefoon = fixture.Create<uint>().ToString(),
                    Website = $"https://{fixture.Create<string>()}.vlaanderen",
                    SocialMedia = $"https://{fixture.Create<string>()}.vlaanderen",
                    PrimairContactInfo = false,
                }
            ).OmitAutoProperties());
    }


    public static void CustomizeEventContactInfo(this IFixture fixture)
    {
        fixture.Customize<AssociationRegistry.Events.CommonEventDataTypes.ContactInfo>(
            composer => composer.FromFactory(
                    () => new AssociationRegistry.Events.CommonEventDataTypes.ContactInfo(
                        fixture.Create<string>(),
                        $"a{fixture.Create<string>()}@example.org",
                        fixture.Create<uint>().ToString(),
                        $"https://{fixture.Create<string>()}.vlaanderen",
                        $"https://{fixture.Create<string>()}.vlaanderen",
                        false
                    )
                ).OmitAutoProperties());
    }
}

public class ImmutableArraySpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var t = request as Type;
        if (t == null)
        {
            return new NoSpecimen();
        }

        var typeArguments = t.GetGenericArguments();
        if (typeArguments.Length != 1)
        {
            return new NoSpecimen();
        }

        if (typeof(ImmutableArray<>) == t.GetGenericTypeDefinition())
        {
            dynamic list = context.Resolve(typeof(IList<>).MakeGenericType(typeArguments));
            return ImmutableArray.ToImmutableArray(list);
        }

        return new NoSpecimen();
    }
}
