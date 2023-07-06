namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using System;
using Events;

public record FeitelijkeVerenigingWerdGeregistreerdData(
    string VCode,
    string Naam,
    string KorteNaam,
    string KorteBeschrijving,
    Registratiedata.Doelgroep Doelgroep,
    bool IsUitgeschrevenUitPubliekeDatastroom,
    DateOnly? Startdatum,
    Registratiedata.Contactgegeven[] Contactgegevens,
    Registratiedata.Locatie[] Locaties,
    FeitelijkeVerenigingWerdGeregistreerdData.Vertegenwoordiger[] Vertegenwoordigers,
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket)
{
    public static FeitelijkeVerenigingWerdGeregistreerdData Create(FeitelijkeVerenigingWerdGeregistreerd e)
        => new(
            e.VCode,
            e.Naam,
            e.KorteNaam,
            e.KorteBeschrijving,
            e.Doelgroep,
            e.IsUitgeschrevenUitPubliekeDatastroom,
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
        public static Vertegenwoordiger With(Registratiedata.Vertegenwoordiger vertegenwoordiger)
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
