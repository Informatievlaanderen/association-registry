namespace AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

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
    ///     - Correspondentie - Slechts één maal mogelijk<br />
    /// </summary>
    [DataMember(Name = "Locatietype")]
    public string Locatietype { get; init; } = null!;

    /// <summary>Duidt aan dat dit de hoofdlocatie is</summary>
    [DataMember(Name = "Hoofdlocatie")]
    public bool Hoofdlocatie { get; init; }

    /// <summary>Een standaard geformatteerde weergave van het adres van de locatie</summary>
    [DataMember(Name = "Adres")]
    public string Adres { get; init; } = null!;

    /// <summary>Een beschrijvende naam voor de locatie</summary>
    [DataMember(Name = "Naam")]
    public string? Naam { get; init; }

    /// <summary>De straat van de locatie</summary>
    [DataMember(Name = "Straatnaam")]
    public string Straatnaam { get; init; } = null!;

    /// <summary>Het huisnummer van de locatie</summary>
    [DataMember(Name = "Huisnummer")]
    public string Huisnummer { get; init; } = null!;

    /// <summary>Het busnummer van de locatie</summary>
    [DataMember(Name = "Busnummer")]
    public string? Busnummer { get; init; }

    /// <summary>De postcode van de locatie</summary>
    [DataMember(Name = "Postcode")]
    public string Postcode { get; init; } = null!;

    /// <summary>De gemeente van de locatie</summary>
    [DataMember(Name = "Gemeente")]
    public string Gemeente { get; init; } = null!;

    /// <summary>Het land van de locatie</summary>
    [DataMember(Name = "Land")]
    public string Land { get; init; } = null!;
}
