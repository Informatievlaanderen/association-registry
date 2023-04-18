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
    string KorteNaam,
    string KorteBeschrijving,
    DateOnly? Startdatum,
    string KboNummer,
    VerenigingWerdGeregistreerd.Contactgegeven[] Contactgegevens,
    VerenigingWerdGeregistreerd.Locatie[] Locaties,
    VerenigingWerdGeregistreerd.Vertegenwoordiger[] Vertegenwoordigers,
    VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent
{
    public record Contactgegeven(
        int ContactgegevenId,
        string Type,
        string Waarde,
        string Beschrijving,
        bool IsPrimair)
    {
        public static Contactgegeven With(Vereniging.Contactgegeven contactgegeven)
            => new(
                contactgegeven.ContactgegevenId,
                contactgegeven.Type,
                contactgegeven.Waarde,
                contactgegeven.Beschrijving,
                contactgegeven.IsPrimair);
    }

    public record Locatie(
        string Naam,
        string Straatnaam,
        string Huisnummer,
        string Busnummer,
        string Postcode,
        string Gemeente,
        string Land,
        bool Hoofdlocatie,
        string Locatietype)
    {
        public static Locatie With(Vereniging.Locatie locatie)
            => new(
                locatie.Naam ?? string.Empty,
                locatie.Straatnaam,
                locatie.Huisnummer,
                locatie.Busnummer ?? string.Empty,
                locatie.Postcode,
                locatie.Gemeente,
                locatie.Land,
                locatie.Hoofdlocatie,
                locatie.Locatietype);
    }

    public record Vertegenwoordiger(
        string Insz,
        bool PrimairContactpersoon,
        string Roepnaam,
        string Rol,
        string Voornaam,
        string Achternaam,
        string Email,
        string Telefoon,
        string Mobiel,
        string SocialMedia)
    {
        public static Vertegenwoordiger With(Vereniging.Vertegenwoordiger vertegenwoordiger)
            => new(
                vertegenwoordiger.Insz,
                vertegenwoordiger.PrimairContactpersoon,
                vertegenwoordiger.Roepnaam ?? string.Empty,
                vertegenwoordiger.Rol ?? string.Empty,
                vertegenwoordiger.Voornaam,
                vertegenwoordiger.Achternaam,
                vertegenwoordiger.Email.Waarde,
                vertegenwoordiger.TelefoonNummer.Waarde,
                vertegenwoordiger.Mobiel.Waarde,
                vertegenwoordiger.SocialMedia.Waarde);
    }

    public record HoofdactiviteitVerenigingsloket(
        string Code,
        string Beschrijving)
    {
        public static HoofdactiviteitVerenigingsloket With(Vereniging.HoofdactiviteitVerenigingsloket activiteit)
            => new(activiteit.Code, activiteit.Beschrijving);
    }
}
