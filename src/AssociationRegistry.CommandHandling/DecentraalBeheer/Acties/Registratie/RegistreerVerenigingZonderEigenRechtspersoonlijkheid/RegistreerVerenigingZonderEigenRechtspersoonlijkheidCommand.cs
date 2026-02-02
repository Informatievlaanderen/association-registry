namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;

using System.Text;
using AssociationRegistry.DecentraalBeheer.Vereniging;

public record RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand(
    object OriginalRequest,
    VerenigingsNaam Naam,
    string? KorteNaam,
    string? KorteBeschrijving,
    Datum? Startdatum,
    Doelgroep Doelgroep,
    bool IsUitgeschrevenUitPubliekeDatastroom,
    Contactgegeven[] Contactgegevens,
    Locatie[] Locaties,
    Vertegenwoordiger[] Vertegenwoordigers,
    HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket,
    Werkingsgebied[] Werkingsgebieden,
    string Bevestigingstoken = ""
)
{
    public bool HeeftBevestigingstoken => !string.IsNullOrWhiteSpace(Bevestigingstoken);

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"Naam = {Naam}, ");
        builder.Append($"KorteNaam = {KorteNaam}, ");
        builder.Append($"KorteBeschrijving = {KorteBeschrijving}, ");
        builder.Append($"Startdatum = {Startdatum}, ");
        builder.Append($"Doelgroep = {Doelgroep}, ");
        builder.Append($"IsUitgeschrevenUitPubliekeDatastroom = {IsUitgeschrevenUitPubliekeDatastroom}, ");
        builder.Append($"Contactgegevens = {Contactgegevens.Length} items, ");
        builder.Append($"Locaties = {Locaties.Length} items, ");
        builder.Append($"Vertegenwoordigers = {Vertegenwoordigers.Length} items, ");
        builder.Append($"HoofdactiviteitenVerenigingsloket = {HoofdactiviteitenVerenigingsloket.Length} items, ");
        builder.Append($"Werkingsgebieden = {Werkingsgebieden.Length} items, ");
        builder.Append($"Bevestigingstoken = {(HeeftBevestigingstoken ? "[PRESENT]" : "[EMPTY]")}");
        return true;
    }
}
