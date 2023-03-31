namespace AssociationRegistry.Events;

using Framework;

/// <summary>
/// </summary>
/// <param name="VCode"></param>
/// <param name="Naam"></param>
/// <param name="KorteNaam"></param>
/// <param name="KorteBeschrijving"></param>
/// <param name="Startdatum"></param>
/// <param name="KboNummer"></param>
/// <param name="Status"></param>
/// <param name="DatumLaatsteAanpassing"></param>
/// <param name="Inititator"></param>
public record VerenigingWerdGeregistreerd(
    string VCode,
    string Naam,
    string? KorteNaam,
    string? KorteBeschrijving,
    DateOnly? Startdatum,
    string? KboNummer,
    VerenigingWerdGeregistreerd.Contactgegeven[] Contactgegevens,
    VerenigingWerdGeregistreerd.Locatie[] Locaties,
    VerenigingWerdGeregistreerd.Vertegenwoordiger[] Vertegenwoordigers,
    VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent
{
    public record Contactgegeven(
        int ContactgegevenId,
        ContactGegevens.ContactgegevenType Type,
        string Waarde,
        string Omschrijving,
        bool IsPrimair);

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
        string Voornaam,
        string Achternaam,
        Contactgegeven[] Contactgegevens);

    public record HoofdactiviteitVerenigingsloket(
        string Code,
        string Beschrijving);
}
