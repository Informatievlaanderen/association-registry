namespace AssociationRegistry.Test.Common.AutoFixture;

using Acties.RegistreerFeitelijkeVereniging;
using Acties.WijzigLidmaatschap;
using global::AutoFixture;
using Vereniging;

public static class CommandCustomizations
{
    public static void CustomizeCommands(Fixture fixture)
    {
        fixture.CustomizeRegistreerFeitelijkeVerenigingCommand();
        fixture.CustomizeTewijzigenLidmaatschap();
    }

    public static void CustomizeRegistreerFeitelijkeVerenigingCommand(this IFixture fixture, bool withoutWerkingsgebieden = false)
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
                                                                 fixture.CreateMany<HoofdactiviteitVerenigingsloket>().DistinctBy(x => x.Code).ToArray(),
                                                                 withoutWerkingsgebieden
                                                                     ? []
                                                                     : fixture.CreateMany<Werkingsgebied>().Distinct().ToArray(),
                                                                 SkipDuplicateDetection: true)
                                                         )
                                                        .OmitAutoProperties());
    }

    private static void CustomizeTewijzigenLidmaatschap(this IFixture fixture)
    {
        fixture.Customize<WijzigLidmaatschapCommand.TeWijzigenLidmaatschap>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () =>
                                                             {
                                                                 var geldigVan = fixture.Create<GeldigVan>();

                                                                 var geldigTot =
                                                                     new GeldigTot(geldigVan.DateOnly.Value.AddDays(fixture.Create<int>()));

                                                                 return new WijzigLidmaatschapCommand.TeWijzigenLidmaatschap(
                                                                     fixture.Create<LidmaatschapId>(),
                                                                     geldigVan,
                                                                     geldigTot,
                                                                     fixture.Create<LidmaatschapIdentificatie>(),
                                                                     fixture.Create<LidmaatschapBeschrijving>()
                                                                 );
                                                             })
                                                        .OmitAutoProperties());
    }
}
