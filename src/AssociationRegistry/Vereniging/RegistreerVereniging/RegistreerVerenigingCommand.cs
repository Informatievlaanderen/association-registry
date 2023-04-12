namespace AssociationRegistry.Vereniging.RegistreerVereniging;

using ContactGegevens;
using Primitives;
using Startdatums;

public record RegistreerVerenigingCommand(
    string Naam,
    string? KorteNaam,
    string? KorteBeschrijving,
    Startdatum Startdatum,
    string? KboNumber,
    RegistreerVerenigingCommand.Contactgegeven[] Contactgegevens,
    RegistreerVerenigingCommand.Locatie[] Locaties,
    RegistreerVerenigingCommand.Vertegenwoordiger[] Vertegenwoordigers,
    string[] HoofdactiviteitenVerenigingsloket,
    bool SkipDuplicateDetection = false)
{
    public record Locatie(
        string? Naam,
        string Straatnaam,
        string Huisnummer,
        string? Busnummer,
        string Postcode,
        string Gemeente,
        string Land,
        bool Hoofdlocatie,
        string Locatietype);

    public record Vertegenwoordiger(
        string Insz,
        bool PrimairContactpersoon,
        string? Roepnaam,
        string? Rol,
        Contactgegeven[] Contactgegevens);

    public record Contactgegeven(
        ContactgegevenType Type,
        string Waarde,
        string? Omschrijving,
        bool IsPrimair);
}
