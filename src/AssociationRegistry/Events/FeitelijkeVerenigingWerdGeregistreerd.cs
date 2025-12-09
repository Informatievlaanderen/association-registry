namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

public interface IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
{
    Bron Bron { get; }
    string VCode { get; init; }
    string Naam { get; init; }
    string KorteNaam { get; init; }
    string KorteBeschrijving { get; init; }
    DateOnly? Startdatum { get; init; }
    Registratiedata.Doelgroep Doelgroep { get; init; }
    bool IsUitgeschrevenUitPubliekeDatastroom { get; init; }
    Registratiedata.Contactgegeven[] Contactgegevens { get; init; }
    Registratiedata.Locatie[] Locaties { get; init; }
    Registratiedata.Vertegenwoordiger[] Vertegenwoordigers { get; init; }
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; }
}

[Obsolete("These are the upcasted events, you might be looking for <EventName>+ZonderPersoonsgegevens")]
public record FeitelijkeVerenigingWerdGeregistreerd(
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
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent, IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
}

public interface IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens
{
    Bron Bron { get; }
    string VCode { get; init; }
    string Naam { get; init; }
    string KorteNaam { get; init; }
    string KorteBeschrijving { get; init; }
    DateOnly? Startdatum { get; init; }
    Registratiedata.Doelgroep Doelgroep { get; init; }
    bool IsUitgeschrevenUitPubliekeDatastroom { get; init; }
    Registratiedata.Contactgegeven[] Contactgegevens { get; init; }
    Registratiedata.Locatie[] Locaties { get; init; }
    Registratiedata.VertegenwoordigerZonderPersoonsgegevens[] Vertegenwoordigers { get; init; }
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; }
}

public record FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens(
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
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent, IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
}
