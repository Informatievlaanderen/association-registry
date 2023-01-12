namespace AssociationRegistry.Vereniging;

using System;

public record RegistreerVerenigingCommand(
    string Naam,
    string? KorteNaam,
    string? KorteBeschrijving,
    DateOnly? Startdatum,
    string? KboNummber,
    IEnumerable<RegistreerVerenigingCommand.ContactInfo>? ContactInfoLijst,
    IEnumerable<RegistreerVerenigingCommand.Locatie>? Locaties)
{
    public record ContactInfo(
        string? Contactnaam,
        string? Email,
        string? Telefoon,
        string? Website,
        string? SocialMedia);

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
}
