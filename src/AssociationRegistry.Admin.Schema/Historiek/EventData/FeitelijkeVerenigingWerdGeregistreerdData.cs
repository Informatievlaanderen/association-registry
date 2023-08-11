namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using System;
using System.Runtime.Serialization;
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
    VertegenwoordigerData[] Vertegenwoordigers,
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
            e.Vertegenwoordigers.Select(VertegenwoordigerData.Create).ToArray(),
            e.HoofdactiviteitenVerenigingsloket
        );
}
