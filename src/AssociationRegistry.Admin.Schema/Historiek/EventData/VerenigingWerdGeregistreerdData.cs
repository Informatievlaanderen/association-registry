namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using Events;

public record VerenigingWerdGeregistreerdData(
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
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket,
    Registratiedata.Bankrekeningnummer[] Bankrekeningnummers
    )
{
    public static VerenigingWerdGeregistreerdData Create(IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd e)
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
            e.HoofdactiviteitenVerenigingsloket,
            e.Bankrekeningnummers
        );
}
