namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using System;
using System.Runtime.Serialization;

[DataContract]
public class Vereniging
{
    /// <summary>De vCode van de vereniging</summary>
    [DataMember(Name = "VCode")]
    public string VCode { get; init; } = null!;

    /// <summary>Het type van de vereniging</summary>
    [DataMember(Name = "Type")]
    public VerenigingsType Type { get; init; } = null!;

    /// <summary>De naam van de vereniging</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;

    /// <summary>De korte naam van de vereniging</summary>
    [DataMember(Name = "KorteNaam")]
    public string KorteNaam { get; init; } = null!;

    /// <summary>De lijst van hoofdactiviteiten erkend door het vereningingsloket</summary>
    [DataMember(Name = "HoofdactiviteitenVerenigingsloket")]
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; } = null!;

    /// <summary>De doelgroep waarop de vereniging zich richt</summary>
    [DataMember(Name = "Doelgroep")]
    public string Doelgroep { get; init; } = null!;

    /// <summary>De locaties waar de vereniging actief is</summary>
    [DataMember(Name = "Locaties")]
    public Locatie[] Locaties { get; init; } = null!;

    /// <summary>De activiteiten die de vereniging uitvoert</summary>
    [DataMember(Name = "Activiteiten")]
    public Activiteit[] Activiteiten { get; init; } = null!;

    /// <summary>Weblinks i.v.m. deze vereniging</summary>
    [DataMember(Name = "Links")]
    public VerenigingLinks Links { get; init; } = null!;
}

[DataContract]
public class Locatie
{
    /// <summary>
    /// Waarvoor deze locatie gebruikt wordt
    /// </summary>
    [DataMember(Name = "Locatietype")]
    public string Locatietype { get; init; } = null!;

    /// <summary>
    /// Is dit de hoofdlocatie van deze vereniging
    /// </summary>
    [DataMember(Name = "Hoofdlocatie")]
    public bool Hoofdlocatie { get; init; }

    /// <summary>
    /// Het volledige adres van de vereniging
    /// </summary>
    [DataMember(Name = "Adres")]
    public string Adres { get; init; } = null!;

    ///<summary>
    /// De naam waarmee deze vereniging deze locatie herkent"
    /// </summary>
    [DataMember(Name = "Naam")]
    public string? Naam { get; init; }

    /// <summary>
    /// De postcode van de locatie
    /// </summary>
    [DataMember(Name = "Postcode")]
    public string Postcode { get; init; } = null!;

    /// <summary>
    /// De gemeente waarin de locatie ligt
    /// </summary>
    [DataMember(Name = "Gemeente")]
    public string Gemeente { get; init; } = null!;
}

[DataContract]
public class Activiteit
{
    /// <summary>
    /// Het id van deze activiteit
    /// </summary>
    [DataMember(Name = "Id")]
    public int Id { get; init; }

    /// <summary>
    /// De categorie van deze activiteit
    /// </summary>
    [DataMember(Name = "Categorie")]
    public string Categorie { get; init; } = null!;
}

[DataContract]
public class VerenigingsType
{
    /// <summary>
    /// De code van het type vereniging
    /// </summary>
    [DataMember]
    public string Code { get; set; } = null!;

    /// <summary>
    /// De beschrijving van het type vereniging
    /// </summary>
    [DataMember]
    public string Beschrijving { get; set; } = null!;
}

[DataContract]
public class HoofdactiviteitVerenigingsloket
{
    /// <summary>
    /// De verkorte code van de hoofdactiviteit
    /// </summary>
    [DataMember]
    public string Code { get; set; } = null!;

    /// <summary>
    /// De volledige beschrijving van de hoofdactiviteit
    /// </summary>
    [DataMember]
    public string Beschrijving { get; set; } = null!;
}

[DataContract]
public class VerenigingLinks
{
    /// <summary>
    /// De link naar het publiek detail van de vereniging
    /// </summary>
    [DataMember(Name = "Detail")]
    public Uri Detail { get; init; } = null!;
}
