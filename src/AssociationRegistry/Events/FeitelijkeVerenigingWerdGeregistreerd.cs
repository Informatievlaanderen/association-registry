namespace AssociationRegistry.Events;

using Framework;

public record FeitelijkeVerenigingWerdGeregistreerd(
    string VCode,
    string Naam,
    string KorteNaam,
    string KorteBeschrijving,
    DateOnly? Startdatum,
    FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven[] Contactgegevens,
    FeitelijkeVerenigingWerdGeregistreerd.Locatie[] Locaties,
    FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger[] Vertegenwoordigers,
    FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent
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
        int VertegenwoordigerId,
        string Insz,
        bool IsPrimair,
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
                vertegenwoordiger.VertegenwoordigerId,
                vertegenwoordiger.Insz,
                vertegenwoordiger.IsPrimair,
                vertegenwoordiger.Roepnaam ?? string.Empty,
                vertegenwoordiger.Rol ?? string.Empty,
                vertegenwoordiger.Voornaam,
                vertegenwoordiger.Achternaam,
                vertegenwoordiger.Email.Waarde,
                vertegenwoordiger.Telefoon.Waarde,
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
