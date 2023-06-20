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

    /// <summary>Duidt aan dat dit de hoofdlocatie is</summary>
    [DataMember]
    public bool Hoofdlocatie { get; set; }

    /// <summary>Een beschrijvende naam voor de locatie</summary>
    [DataMember]
    public string? Naam { get; set; }

    /// <summary>Het adres van de locatie</summary>
    [DataMember]
    public ToeTeVoegenAdres Adres { get; set; } = new();

    public static Locatie Map(ToeTeVoegenLocatie loc)
        => Locatie.Create(
            loc.Naam,
            loc.Adres.Straatnaam,
            loc.Adres.Huisnummer,
            loc.Adres.Busnummer,
            loc.Adres.Postcode,
            loc.Adres.Gemeente,
            loc.Adres.Land,
            loc.Hoofdlocatie,
            loc.Locatietype);
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
