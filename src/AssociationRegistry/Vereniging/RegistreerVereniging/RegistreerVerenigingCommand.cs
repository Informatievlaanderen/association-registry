namespace AssociationRegistry.Vereniging.RegistreerVereniging;

using ContactGegevens;
using Hoofdactiviteiten;
using KboNummers;
using Locaties;
using Primitives;
using Startdatums;
using VerenigingsNamen;
using Vertegenwoordigers;

public record RegistreerVerenigingCommand(
    VerenigingsNaam Naam,
    string? KorteNaam,
    string? KorteBeschrijving,
    Startdatum? Startdatum,
    KboNummer? KboNumber,
    Contactgegeven[] Contactgegevens,
    LocatieLijst Locaties,
    RegistreerVerenigingCommand.TeRegistrerenVertegenwoordiger[] Vertegenwoordigers,
    HoofdactiviteitenVerenigingsloketLijst HoofdactiviteitenVerenigingsloket,
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

    public record TeRegistrerenVertegenwoordiger(
        string Insz,
        bool PrimairContactpersoon,
        string? Roepnaam,
        string? Rol,
        ContactGegevens.Contactgegeven[] Contactgegevens);

    public record Contactgegeven(
        ContactgegevenType Type,
        string Waarde,
        string? Omschrijving,
        bool IsPrimair);
}
