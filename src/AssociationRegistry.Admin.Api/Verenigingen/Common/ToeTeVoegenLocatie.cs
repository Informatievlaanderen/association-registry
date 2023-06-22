namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using System.Runtime.Serialization;
using Vereniging;

/// <summary>Een locatie van een vereniging</summary>
[DataContract]
public class ToeTeVoegenLocatie
{
    /// <summary>
    ///     Het soort locatie dat beschreven wordt<br />
    ///     <br />
    ///     Mogelijke waarden:<br />
    ///     - Activiteiten<br />
    ///     - Correspondentie - Slecht één maal mogelijk<br />
    /// </summary>
    [DataMember]
    public string Locatietype { get; set; } = null!;

    /// <summary>Duidt aan dat dit de primaire locatie is</summary>
    [DataMember]
    public bool IsPrimair { get; set; }

    /// <summary>Een beschrijvende naam voor de locatie</summary>
    [DataMember]
    public string? Naam { get; set; }

    /// <summary>De unieke identificator van het adres in een andere bron</summary>
    [DataMember]
    public AdresId? AdresId { get; set; }

    /// <summary>Het adres van de locatie</summary>
    [DataMember]
    public ToeTeVoegenAdres? Adres { get; set; }

    public static Locatie Map(ToeTeVoegenLocatie loc)
        => Locatie.Create(
            loc.Naam,
            loc.IsPrimair,
            loc.Locatietype,
            loc.AdresId is not null ? AssociationRegistry.Vereniging.AdresId.Create(loc.AdresId.Broncode, loc.AdresId.Bronwaarde) : null,
            loc.Adres is not null
                ? AssociationRegistry.Vereniging.Adres.Create(
                    loc.Adres.Straatnaam,
                    loc.Adres.Huisnummer,
                    loc.Adres.Busnummer,
                    loc.Adres.Postcode,
                    loc.Adres.Gemeente,
                    loc.Adres.Land)
                : null);
}

/// <summary>Een adres van een locatie</summary>
[DataContract]
public class ToeTeVoegenAdres
{
    /// <summary>De straat van het adres</summary>
    [DataMember]
    public string Straatnaam { get; set; } = null!;

    /// <summary>Het huisnummer van het adres</summary>
    [DataMember]
    public string Huisnummer { get; set; } = null!;

    /// <summary>Het busnummer van het adres</summary>
    [DataMember]
    public string? Busnummer { get; set; }

    /// <summary>De postcode van het adres</summary>
    [DataMember]
    public string Postcode { get; set; } = null!;

    /// <summary>De gemeente van het adres</summary>
    [DataMember]
    public string Gemeente { get; set; } = null!;

    /// <summary>Het land van het adres</summary>
    [DataMember]
    public string Land { get; set; } = null!;
}

/// <summary>De unieke identificator van het adres in een andere bron</summary>
[DataContract]
public class AdresId
{
    /// <summary>De code van de bron van het adres</summary>
    [DataMember]
    public string Broncode { get; set; } = null!;

    /// <summary>De unieke identificator volgens de bron</summary>
    [DataMember]
    public string Bronwaarde { get; set; } = null!;
}
