namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens(
    string VCode,
    string Naam,
    string KorteNaam,
    string KorteBeschrijving,
    DateOnly? Startdatum,
    Registratiedata.Doelgroep Doelgroep,
    bool IsUitgeschrevenUitPubliekeDatastroom,
    Registratiedata.Contactgegeven[] Contactgegevens,
    Registratiedata.Locatie[] Locaties,
    Registratiedata.VertegenwoordigerZonderPersoonsgegevens[] Vertegenwoordigers,
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket,
    Registratiedata.DuplicatieInfo? DuplicatieInfo
) : IEvent, IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;
}

[Obsolete("These are the upcasted events, only use this in projections and State")]
public record VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
    string VCode,
    string Naam,
    string KorteNaam,
    string KorteBeschrijving,
    DateOnly? Startdatum,
    Registratiedata.Doelgroep Doelgroep,
    bool IsUitgeschrevenUitPubliekeDatastroom,
    Registratiedata.Contactgegeven[] Contactgegevens,
    Registratiedata.Locatie[] Locaties,
    Registratiedata.Vertegenwoordiger[] Vertegenwoordigers,
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket,
    Registratiedata.DuplicatieInfo? DuplicatieInfo
) : IEvent, IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;

    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        builder.Append($"Naam = {Naam}, ");
        builder.Append($"KorteNaam = {KorteNaam}, ");
        builder.Append($"KorteBeschrijving = {KorteBeschrijving}, ");
        builder.Append($"Startdatum = {Startdatum}, ");
        builder.Append($"Doelgroep = {Doelgroep}, ");
        builder.Append($"IsUitgeschrevenUitPubliekeDatastroom = {IsUitgeschrevenUitPubliekeDatastroom}, ");
        builder.Append($"Contactgegevens = {Contactgegevens.Length} items, ");
        builder.Append($"Locaties = {Locaties.Length} items, ");
        builder.Append($"Vertegenwoordigers = {Vertegenwoordigers.Length} items, ");
        builder.Append($"HoofdactiviteitenVerenigingsloket = {HoofdactiviteitenVerenigingsloket.Length} items, ");
        builder.Append($"DuplicatieInfo = {DuplicatieInfo}");
        return true;
    }
}
