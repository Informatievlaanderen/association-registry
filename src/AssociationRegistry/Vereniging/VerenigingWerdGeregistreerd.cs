﻿namespace AssociationRegistry.Vereniging;

using ContactInfo;
using Framework;

/// <summary>
///
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
    VerenigingWerdGeregistreerd.ContactInfo[]? Contacten,
    VerenigingWerdGeregistreerd.Locatie[]? Locaties,
    DateOnly? DatumLaatsteAanpassing) : IEvent
{
    public record ContactInfo(
        string? Contactnaam,
        string? Email,
        string? Telefoon,
        string? Website,
        string? SocialMedia)
    {
        public static ContactInfo[] FromContacten(ContactLijst contactLijst)
            => contactLijst.Select(c => new ContactInfo(c.Contactnaam, c.Email, c.Telefoon, c.Website, c.SocialMedia)).ToArray();
    }

    public record Locatie(
        string? Naam,
        string Straatnaam,
        string Huisnummer,
        string? Busnummer,
        string Postcode,
        string Gemeente,
        string Land,
        bool HoofdLocatie,
        string LocatieType);
}
