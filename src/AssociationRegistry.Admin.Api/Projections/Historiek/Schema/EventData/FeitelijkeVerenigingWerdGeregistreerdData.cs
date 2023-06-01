namespace AssociationRegistry.Admin.Api.Projections.Historiek.Schema.EventData;

using System;
using System.Linq;
using Events;

public record FeitelijkeVerenigingWerdGeregistreerdData(
    string VCode,
    string Naam,
    string KorteNaam,
    string KorteBeschrijving,
    DateOnly? Startdatum,
    FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven[] Contactgegevens,
    FeitelijkeVerenigingWerdGeregistreerd.Locatie[] Locaties,
    FeitelijkeVerenigingWerdGeregistreerdData.Vertegenwoordiger[] Vertegenwoordigers,
    FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket)
{
    public static FeitelijkeVerenigingWerdGeregistreerdData Create(FeitelijkeVerenigingWerdGeregistreerd e)
        => new(
            e.VCode,
            e.Naam,
            e.KorteNaam,
            e.KorteBeschrijving,
            e.Startdatum,
            e.Contactgegevens,
            e.Locaties,
            e.Vertegenwoordigers.Select(Vertegenwoordiger.With).ToArray(),
            e.HoofdactiviteitenVerenigingsloket
        );


    public record Vertegenwoordiger(
        int VertegenwoordigerId,
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
        public static Vertegenwoordiger With(FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger vertegenwoordiger)
            => new(
                vertegenwoordiger.VertegenwoordigerId,
                vertegenwoordiger.IsPrimair,
                vertegenwoordiger.Roepnaam,
                vertegenwoordiger.Rol,
                vertegenwoordiger.Voornaam,
                vertegenwoordiger.Achternaam,
                vertegenwoordiger.Email,
                vertegenwoordiger.Telefoon,
                vertegenwoordiger.Mobiel,
                vertegenwoordiger.SocialMedia);
    }
}
