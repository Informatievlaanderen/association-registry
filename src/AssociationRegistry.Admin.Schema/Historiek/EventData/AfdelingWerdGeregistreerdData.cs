namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using System;
using Events;

public record AfdelingWerdGeregistreerdData(
    string VCode,
    string Naam,
    string KboNummerMoedervereniging,
    string KorteNaam,
    string KorteBeschrijving,
    Registratiedata.Doelgroep Doelgroep,
    DateOnly? Startdatum,
    Registratiedata.Contactgegeven[] Contactgegevens,
    Registratiedata.Locatie[] Locaties,
    AfdelingWerdGeregistreerdData.Vertegenwoordiger[] Vertegenwoordigers,
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket)
{
    public static AfdelingWerdGeregistreerdData Create(AfdelingWerdGeregistreerd e)
        => new(
            e.VCode,
            e.Naam,
            e.Moedervereniging.KboNummer,
            e.KorteNaam,
            e.KorteBeschrijving,
            e.Doelgroep,
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
