namespace AssociationRegistry.Test.Admin.Api.Framework;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using VCodes;
using AutoFixture;
using AutoFixture.Dsl;
using Events;
using NodaTime;

public static class AutoFixtureCustomizations
{
    public static Fixture CustomizeAll(this Fixture fixture)
    {
        fixture.CustomizeDateOnly();
        fixture.CustomizeVCode();
        fixture.CustomizeInstant();
        fixture.CustomizeRegistreerVerenigingRequestLocatie();
        fixture.CustomizeVerenigingWerdGeregistreerdLocatie();
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
}
