namespace AssociationRegistry.Test.Framework.Customizations;

using Acties.RegistreerFeitelijkeVereniging;
using Acties.VoegContactgegevenToe;
using AutoFixture;
using Vereniging;

public static class CommandCustomizations
{
    public static void CustomizeCommands(Fixture fixture)
    {
        fixture.CustomizeRegistreerFeitelijkeVerenigingCommand();
        fixture.CustomizeVoegContactgegevenToeCommand();
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

    private static void CustomizeVoegContactgegevenToeCommand(this IFixture fixture)
    {
        fixture.Customize<VoegContactgegevenToeCommand>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => new VoegContactgegevenToeCommand(
                                                                 VCode: fixture.Create<VCode>(),
                                                                 Contactgegeven: fixture.Create<Contactgegeven>())
                                                         )
                                                        .OmitAutoProperties());
    }
}
