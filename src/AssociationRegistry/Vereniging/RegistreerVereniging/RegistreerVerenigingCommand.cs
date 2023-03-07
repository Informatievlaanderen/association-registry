namespace AssociationRegistry.Vereniging.RegistreerVereniging;

using System;
using Primitives;

public record RegistreerVerenigingCommand(
    string Naam,
    string? KorteNaam,
    string? KorteBeschrijving,
    NullOrEmpty<DateOnly> Startdatum,
    string? KboNummber,
    IEnumerable<RegistreerVerenigingCommand.ContactInfo>? ContactInfoLijst,
    IEnumerable<RegistreerVerenigingCommand.Locatie>? Locaties,
    IEnumerable<RegistreerVerenigingCommand.Vertegenwoordiger>? Vertegenwoordigers,
    IEnumerable<string> HoofdactiviteitenVerenigingsloket,
    bool SkipDuplicateDetection = false)
{
    public record ContactInfo(
        string Contactnaam,
        string? Email,
        string? Telefoon,
        string? Website,
        string? SocialMedia,
        bool PrimairContactInfo);

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
