namespace AssociationRegistry.Test.Common.AutoFixture;

using CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.WijzigLidmaatschap;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using DecentraalBeheer.Vereniging;
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
        fixture.Customize<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand(
                                                                 fixture.Create<VerenigingsNaam>(),
                                                                 fixture.Create<string>(),
                                                                 fixture.Create<string>(),
                                                                 Datum.Create(DateOnly.FromDateTime(DateTime.Today.AddDays(-10))),
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
        fixture.Customize<TeWijzigenLidmaatschap>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () =>
                                                             {
                                                                 var geldigVan = fixture.Create<GeldigVan>();

                                                                 var geldigTot =
                                                                     new GeldigTot(geldigVan.DateOnly.Value.AddDays(fixture.Create<int>()));

                                                                 return new TeWijzigenLidmaatschap(
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
