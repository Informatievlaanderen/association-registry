namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

/// <summary>Een locatie van een vereniging</summary>
[DataContract]
public class Locatie
{
    /// <summary>
    ///     De unieke identificatie code van deze locatie binnen de vereniging
    /// </summary>
    [DataMember(Name = "LocatieId")]
    public int LocatieId { get; set; }

    /// <summary>
    ///     Het soort locatie dat beschreven wordt<br />
    ///     <br />
    ///     Mogelijke waarden:<br />
    ///     - Activiteiten<br />
    ///     - Correspondentie - Slecht één maal mogelijk<br />
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
    public Adres? Adres { get; init; } = null!;

    /// <summary>Een standaard geformatteerde weergave van het adres van de locatie</summary>
    [DataMember(Name = "Adresvoorstelling")]
    public string Adresvoorstelling { get; init; } = null!;

    /// <summary>
    /// De identifier van het adres op een externe bron
    /// </summary>
    [DataMember(Name = "AdresId")]
    public AdresId? AdresId { get; set; }


}
