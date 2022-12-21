namespace AssociationRegistry.Vereniging;

using Contacten;
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
    string? KorteNaam = null,
    string? KorteBeschrijving = null,
    DateOnly? Startdatum = null,
    string? KboNummer = null,
    VerenigingWerdGeregistreerd.ContactInfo[]? Contacten = null,
    DateOnly? DatumLaatsteAanpassing = null) : IEvent
{
    public record ContactInfo(
        string? Contactnaam,
        string? Email,
        string? Telefoon,
        string? Website,
        string? SocialMedia)
    {
        public static ContactInfo[] FromContacten(Contacten contacten)
            => contacten.Select(c => new ContactInfo(c.Contactnaam, c.Email, c.Telefoon, c.Website, c.SocialMedia)).ToArray();

    }
}
