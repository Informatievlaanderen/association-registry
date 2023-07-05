namespace AssociationRegistry.Test.Framework.Customizations;

using Acties.RegistreerAfdeling;
using Acties.RegistreerFeitelijkeVereniging;
using AutoFixture;
using Vereniging;

public static class CommandCustomizations
{
    public static void CustomizeCommands(Fixture fixture)
    {
        fixture.CustomizeRegistreerFeitelijkeVerenigingCommand();
        fixture.CustomizeRegistreerAfdelingCommand();
    }

    private static void CustomizeRegistreerFeitelijkeVerenigingCommand(this IFixture fixture)
    {
        fixture.Customize<RegistreerFeitelijkeVerenigingCommand>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new RegistreerFeitelijkeVerenigingCommand(
                        Naam: fixture.Create<VerenigingsNaam>(),
                        KorteNaam: fixture.Create<string>(),
                        KorteBeschrijving: fixture.Create<string>(),
                        Startdatum: fixture.Create<Startdatum>(),
                        IsUitgeschrevenUitPubliekeDatastroom: false,
                        Contactgegevens: fixture.CreateMany<Contactgegeven>().ToArray(),
                        Locaties: fixture.CreateMany<Locatie>().ToArray(),
                        Vertegenwoordigers: fixture.CreateMany<Vertegenwoordiger>().ToArray(),
                        HoofdactiviteitenVerenigingsloket: fixture.CreateMany<HoofdactiviteitVerenigingsloket>().Distinct().ToArray(),
                        SkipDuplicateDetection: true)
                )
                .OmitAutoProperties());
    }

    private static void CustomizeRegistreerAfdelingCommand(this IFixture fixture)
    {
        fixture.Customize<RegistreerAfdelingCommand>(
            composerTransformation: composer => composer.FromFactory(
                    factory: () => new RegistreerAfdelingCommand(
                        Naam: fixture.Create<VerenigingsNaam>(),
                        KboNummerMoedervereniging: fixture.Create<KboNummer>(),
                        KorteNaam: fixture.Create<string>(),
                        KorteBeschrijving: fixture.Create<string>(),
                        Startdatum: fixture.Create<Startdatum>(),
                        Contactgegevens: fixture.CreateMany<Contactgegeven>().ToArray(),
                        Locaties: fixture.CreateMany<Locatie>().ToArray(),
                        Vertegenwoordigers: fixture.CreateMany<Vertegenwoordiger>().ToArray(),
                        HoofdactiviteitenVerenigingsloket: fixture.CreateMany<HoofdactiviteitVerenigingsloket>().Distinct().ToArray(),
                        SkipDuplicateDetection: true)
                )
                .OmitAutoProperties());
    }
}
