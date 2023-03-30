namespace AssociationRegistry.Events;

using CommonEventDataTypes;
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
    ContactInfo[] ContactInfoLijst,
    VerenigingWerdGeregistreerd.Locatie[] Locaties,
    VerenigingWerdGeregistreerd.Vertegenwoordiger[] Vertegenwoordigers,
    VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent
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
        string Voornaam,
        string Achternaam,
        ContactInfo[] ContactInfoLijst);

    public record HoofdactiviteitVerenigingsloket(
        string Code,
        string Beschrijving);
}
