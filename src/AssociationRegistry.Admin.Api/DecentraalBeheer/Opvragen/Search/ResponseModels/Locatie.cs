namespace AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Locatie
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; set; }

    /// <summary>
    ///     Het soort locatie dat beschreven wordt<br />
    ///     <br />
    ///     Mogelijke waarden:<br />
    ///     - Activiteiten<br />
    ///     - Correspondentie - Slechts één maal mogelijk<br />
    ///     - Maatschappelijke zetel volgens KBO - Enkel mogelijk voor verenigingen met rechtspersoonlijkheid<br />
    /// </summary>
    [DataMember(Name = "Locatietype")]
    public string Locatietype { get; init; } = null!;

    /// <summary>
    /// Is dit de primaire locatie van deze vereniging
    /// </summary>
    [DataMember(Name = "IsPrimair")]
    public bool IsPrimair { get; init; }

    /// <summary>
    /// De volledige adresvoorstelling van de locatie
    /// </summary>
    [DataMember(Name = "Adresvoorstelling")]
    public string Adresvoorstelling { get; init; } = null!;

    ///<summary>
    /// De naam van de locatie
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
