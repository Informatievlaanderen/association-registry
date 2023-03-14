namespace AssociationRegistry.Vereniging.RegistreerVereniging;

using System;
using CommonCommandDataTypes;
using Primitives;

public record RegistreerVerenigingCommand(
    string Naam,
    string? KorteNaam,
    string? KorteBeschrijving,
    NullOrEmpty<DateOnly> Startdatum,
    string? KboNummber,
    IEnumerable<ContactInfo>? ContactInfoLijst,
    IEnumerable<RegistreerVerenigingCommand.Locatie>? Locaties,
    IEnumerable<RegistreerVerenigingCommand.Vertegenwoordiger>? Vertegenwoordigers,
    IEnumerable<string> HoofdactiviteitenVerenigingsloket,
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
        IEnumerable<ContactInfo>? ContactInfoLijst);
}
