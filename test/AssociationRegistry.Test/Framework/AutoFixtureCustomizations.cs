namespace AssociationRegistry.Test.Framework;

using System.Diagnostics.Tracing;
using VCodes;
using AutoFixture;
using AutoFixture.Dsl;
using Be.Vlaanderen.Basisregisters.Utilities;
using ContactInfo.Emails;
using NodaTime;

public static class AutoFixtureCustomizations
{
    public static Fixture CustomizeAll(this Fixture fixture)
    {
        fixture.CustomizeDateOnly();
        fixture.CustomizeVCode();
        fixture.CustomizeInstant();
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

    public static void CustomizeEmail(this IFixture fixture)
    {
        var name = "a" + Guid.NewGuid() + "b";
        var domain = "digitaal";
        var topLevel = "vlaanderen";
        fixture.Customize<Email>(
            customization => customization.FromFactory(
                _ => Email.Create($"{name}@{domain}.{topLevel}")));
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
}
