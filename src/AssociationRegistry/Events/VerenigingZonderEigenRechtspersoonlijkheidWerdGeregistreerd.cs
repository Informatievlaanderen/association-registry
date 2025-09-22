namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd : IEvent, IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(string vCode,
                                                                       string naam,
                                                                       string korteNaam,
                                                                       string korteBeschrijving,
                                                                       DateOnly? startdatum,
                                                                       Registratiedata.Doelgroep ddoelgroep,
                                                                       bool isUitgeschrevenUitPubliekeDatastroom,
                                                                       Registratiedata.Contactgegeven[] contactgegevens,
                                                                       Registratiedata.Locatie[] locaties,
                                                                       Registratiedata.Vertegenwoordiger[] vertegenwoordigers,
                                                                       Registratiedata.HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloket,
                                                                       Registratiedata.DuplicatieInfo? duplicatieInfo = null)
    {
        VCode = vCode;
        Naam = naam;
        KorteNaam = korteNaam;
        KorteBeschrijving = korteBeschrijving;
        Startdatum = startdatum;
        Doelgroep = ddoelgroep;
        IsUitgeschrevenUitPubliekeDatastroom = isUitgeschrevenUitPubliekeDatastroom;
        Contactgegevens = contactgegevens;
        Locaties = locaties;
        Vertegenwoordigers = vertegenwoordigers;
        HoofdactiviteitenVerenigingsloket = hoofdactiviteitenVerenigingsloket;
        DuplicatieInfo = duplicatieInfo ?? Registratiedata.DuplicatieInfo.Onbekend;
    }

    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;

    public string VCode { get; init; }
    public string Naam { get; init; }
    public string KorteNaam { get; init; }
    public string KorteBeschrijving { get; init; }
    public DateOnly? Startdatum { get; init; }
    public Registratiedata.Doelgroep Doelgroep { get; init; }
    public bool IsUitgeschrevenUitPubliekeDatastroom { get; init; }
    public Registratiedata.Contactgegeven[] Contactgegevens { get; init; }
    public Registratiedata.Locatie[] Locaties { get; init; }
    public Registratiedata.Vertegenwoordiger[] Vertegenwoordigers { get; init; }
    public Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; }
    public Registratiedata.DuplicatieInfo DuplicatieInfo { get; init; }
}
