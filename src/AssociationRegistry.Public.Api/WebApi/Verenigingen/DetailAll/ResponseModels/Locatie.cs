namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.DetailAll.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Locatie : IJsonLd
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; init; }

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

    /// <summary>Duidt aan dat dit de primaire locatie is</summary>
    [DataMember(Name = "IsPrimair")]
    public bool IsPrimair { get; init; }

    /// <summary>Een beschrijvende naam voor de locatie</summary>
    [DataMember(Name = "Naam")]
    public string? Naam { get; init; }

    /// <summary>De adrescomponenten van de locatie</summary>
    [DataMember(Name = "Adres")]
    public Adres? Adres { get; init; }

    /// <summary>Een standaard geformatteerde weergave van het adres van de locatie</summary>
    [DataMember(Name = "Adresvoorstelling")]
    public string Adresvoorstelling { get; init; } = null!;

    /// <summary>
    /// De identifier van het adres op een externe bron
    /// </summary>
    [DataMember(Name = "AdresId")]
    public AdresId? AdresId { get; set; }

    /// <summary>
    ///     De verwijzing naar een adres in het adresregister
    /// </summary>
    [DataMember(Name = "VerwijstNaar")]
    public AdresVerwijzing? VerwijstNaar { get; set; }
}
