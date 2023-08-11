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
    VertegenwoordigerData[] Vertegenwoordigers,
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
            e.Vertegenwoordigers.Select(VertegenwoordigerData.With).ToArray(),
            e.HoofdactiviteitenVerenigingsloket
        );

}
