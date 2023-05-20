namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using System;
using System.Runtime.Serialization;

[DataContract]
public class DetailVerenigingResponse
{
    public DetailVerenigingResponse(
        string context,
        VerenigingDetail verenigingDetail,
        Metadata metadata)
    {
        Context = context;
        VerenigingDetail = verenigingDetail;
        Metadata = metadata;
    }

    /// <summary>De JSON-LD open data context</summary>
    [DataMember(Name = "@context")]
    public string Context { get; init; }

    /// <summary> De vereniging waarvan het detail opgevraagd werd</summary>
    [DataMember(Name = "Vereniging")]
    public VerenigingDetail VerenigingDetail { get; init; }

    /// <summary>De metadata van de vereniging, deze bevat bv de datum van laatste aanpassing</summary>
    [DataMember(Name = "Metadata")]
    public Metadata Metadata { get; init; }
}

/// <summary> De vereniging waarvan het detail opgevraagd werd</summary>
[DataContract]
public class VerenigingDetail
{
    public VerenigingDetail(
        string vCode,
        VerenigingsType type,
        string naam,
        string? korteNaam,
        string? korteBeschrijving,
        DateOnly? startdatum,
        string status,
        Contactgegeven[] contactgegevens,
        Locatie[] locaties,
        HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloket)
    {
        VCode = vCode;
        Type = type;
        Naam = naam;
        KorteNaam = korteNaam;
        KorteBeschrijving = korteBeschrijving;
        Startdatum = startdatum;
        Status = status;
        Contactgegevens = contactgegevens;
        Locaties = locaties;
        HoofdactiviteitenVerenigingsloket = hoofdactiviteitenVerenigingsloket;
    }

    /// <summary>De unieke identificatie code van deze vereniging</summary>
    [DataMember(Name = "VCode")]
    public string VCode { get; init; }

    /// <summary>Type van de vereniging</summary>
    [DataMember(Name = "Type")]
    public VerenigingsType Type { get; init; }

    /// <summary>Naam van de vereniging</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; }

    /// <summary>Korte naam van de vereniging</summary>
    [DataMember(Name = "KorteNaam")]
    public string? KorteNaam { get; init; }

    /// <summary>Korte beschrijving van de vereniging</summary>
    [DataMember(Name = "KorteBeschrijving")]
    public string? KorteBeschrijving { get; init; }

    /// <summary>Datum waarop de vereniging gestart is. Deze datum mag niet later zijn dan vandaag</summary>
    [DataMember(Name = "Startdatum")]
    public DateOnly? Startdatum { get; init; }

    /// <summary>Status van de vereniging</summary>
    [DataMember(Name = "Status")]
    public string Status { get; init; }

    /// <summary>De contactgegevens van deze vereniging</summary>
    [DataMember(Name = "Contactgegevens")]
    public Contactgegeven[] Contactgegevens { get; init; }

    /// <summary>Alle locaties waar deze vereniging actief is</summary>
    [DataMember(Name = "Locaties")]
    public Locatie[] Locaties { get; init; }

    /// <summary>De hoofdactivititeiten van deze vereniging volgens het verenigingsloket</summary>
    [DataMember(Name = "HoofdactiviteitenVerenigingsloket")]
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; }
}

/// <summary>De metadata van de vereniging, deze bevat bv de datum van laatste aanpassing</summary>
public class Metadata
{
    /// <param name="datumLaatsteAanpassing"></param>
    public Metadata(string datumLaatsteAanpassing)
    {
        DatumLaatsteAanpassing = datumLaatsteAanpassing;
    }

    /// <summary>De datum waarop de laatste aanpassing uitgevoerd is op de gegevens van de vereniging</summary>
    public string DatumLaatsteAanpassing { get; init; }
}

/// <summary>
/// Type van een vereniging
/// </summary>
public class VerenigingsType
{
    /// <summary>
    /// Code van het type vereniging
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Beschrijving van het type vereniging
    /// </summary>
    public string Beschrijving { get; set; } = null!;
}

/// <summary>Een locatie van een vereniging</summary>
[DataContract]
public class Locatie
{
    public Locatie(
        string locatietype,
        bool hoofdlocatie,
        string adres,
        string? naam,
        string straatnaam,
        string huisnummer,
        string? busnummer,
        string postcode,
        string gemeente,
        string land)
    {
        Locatietype = locatietype;
        Hoofdlocatie = hoofdlocatie;
        Adres = adres;
        Naam = naam;
        Straatnaam = straatnaam;
        Huisnummer = huisnummer;
        Busnummer = busnummer;
        Postcode = postcode;
        Gemeente = gemeente;
        Land = land;
    }

    /// <summary>
    ///     Het soort locatie dat beschreven wordt<br />
    ///     <br />
    ///     Mogelijke waarden:<br />
    ///     - Activiteiten<br />
    ///     - Correspondentie - Slecht één maal mogelijk<br />
    /// </summary>
    [DataMember(Name = "Locatietype")]
    public string Locatietype { get; init; }

    /// <summary>Duidt aan dat dit de hoofdlocatie is</summary>
    [DataMember(Name = "Hoofdlocatie")]
    public bool Hoofdlocatie { get; init; }

    /// <summary>Een standaard geformatteerde weergave van het adres van de locatie</summary>
    [DataMember(Name = "Adres")]
    public string Adres { get; init; }

    /// <summary>Een beschrijvende naam voor de locatie</summary>
    [DataMember(Name = "Naam")]
    public string? Naam { get; init; }

    /// <summary>De straat van de locatie</summary>
    [DataMember(Name = "Straatnaam")]
    public string Straatnaam { get; init; }

    /// <summary>Het huisnummer van de locatie</summary>
    [DataMember(Name = "Huisnummer")]
    public string Huisnummer { get; init; }

    /// <summary>Het busnummer van de locatie</summary>
    [DataMember(Name = "Busnummer")]
    public string? Busnummer { get; init; }

    /// <summary>De postcode van de locatie</summary>
    [DataMember(Name = "Postcode")]
    public string Postcode { get; init; }

    /// <summary>De gemeente van de locatie</summary>
    [DataMember(Name = "Gemeente")]
    public string Gemeente { get; init; }

    /// <summary>Het land van de locatie</summary>
    [DataMember(Name = "Land")]
    public string Land { get; init; }
}

/// <summary>Een contactgegeven van een vereniging dat publiek beschikbaar gesteld zal worden</summary>
[DataContract]
public class Contactgegeven
{
    public Contactgegeven(
        string type,
        string waarde,
        string beschrijving,
        bool isPrimair)
    {
        Type = type;
        Waarde = waarde;
        Beschrijving = beschrijving;
        IsPrimair = isPrimair;
    }

    /// <summary>Het type contactgegeven</summary>
    [DataMember(Name = "type")]
    public string Type { get; init; }

    /// <summary>De waarde van het contactgegeven</summary>
    [DataMember(Name = "waarde")]
    public string Waarde { get; init; }

    /// <summary>
    /// Vrij veld die het het contactgegeven beschrijft (bijv: algemeen, administratie, ...)
    /// </summary>
    [DataMember(Name = "beschrijving")]
    public string Beschrijving { get; init; }

    /// <summary>Duidt het contactgegeven aan als primair contactgegeven</summary>
    [DataMember(Name = "isPrimair")]
    public bool IsPrimair { get; init; }
}

/// <summary>De hoofdactivititeit van een vereniging volgens het verenigingsloket</summary>
[DataContract]
public class HoofdactiviteitVerenigingsloket
{
    public HoofdactiviteitVerenigingsloket(
        string code,
        string beschrijving)
    {
        Code = code;
        Beschrijving = beschrijving;
    }

    /// <summary>De code van de hoofdactivititeit</summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; }

    /// <summary>De beschrijving van de hoofdactivititeit</summary>
    [DataMember(Name = "Beschrijving")]
    public string Beschrijving { get; init; }
}
