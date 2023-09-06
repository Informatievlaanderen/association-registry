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
                                                                 fixture.Create<VerenigingsNaam>(),
                                                                 fixture.Create<string>(),
                                                                 fixture.Create<string>(),
                                                                 fixture.Create<Datum>(),
                                                                 fixture.Create<Doelgroep>(),
                                                                 IsUitgeschrevenUitPubliekeDatastroom: false,
                                                                 fixture.CreateMany<Contactgegeven>().ToArray(),
                                                                 fixture.CreateMany<Locatie>().ToArray(),
                                                                 fixture.CreateMany<Vertegenwoordiger>().ToArray(),
                                                                 fixture.CreateMany<HoofdactiviteitVerenigingsloket>().Distinct().ToArray(),
                                                                 SkipDuplicateDetection: true)
                                                         )
                                                        .OmitAutoProperties());
    }

    private static void CustomizeRegistreerAfdelingCommand(this IFixture fixture)
    {
        fixture.Customize<RegistreerAfdelingCommand>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => new RegistreerAfdelingCommand(
                                                                 fixture.Create<VerenigingsNaam>(),
                                                                 fixture.Create<KboNummer>(),
                                                                 fixture.Create<string>(),
                                                                 fixture.Create<string>(),
                                                                 fixture.Create<Datum>(),
                                                                 fixture.Create<Doelgroep>(),
                                                                 fixture.CreateMany<Contactgegeven>().ToArray(),
                                                                 fixture.CreateMany<Locatie>().ToArray(),
                                                                 fixture.CreateMany<Vertegenwoordiger>().ToArray(),
                                                                 fixture.CreateMany<HoofdactiviteitVerenigingsloket>().Distinct().ToArray(),
                                                                 SkipDuplicateDetection: true)
                                                         )
                                                        .OmitAutoProperties());
    }
}
