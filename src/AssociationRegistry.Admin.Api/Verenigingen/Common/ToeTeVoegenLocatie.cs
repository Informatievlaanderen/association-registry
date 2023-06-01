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

    /// <summary>De straat van de locatie</summary>
    [DataMember]
    public string Straatnaam { get; set; } = null!;

    /// <summary>Het huisnummer van de locatie</summary>
    [DataMember]
    public string Huisnummer { get; set; } = null!;

    /// <summary>Het busnummer van de locatie</summary>
    [DataMember]
    public string? Busnummer { get; set; }

    /// <summary>De postcode van de locatie</summary>
    [DataMember]
    public string Postcode { get; set; } = null!;

    /// <summary>De gemeente van de locatie</summary>
    [DataMember]
    public string Gemeente { get; set; } = null!;

    /// <summary>Het land van de locatie</summary>
    [DataMember]
    public string Land { get; set; } = null!;

    public static Locatie Map(ToeTeVoegenLocatie loc)
        => Locatie.Create(
            loc.Naam,
            loc.Straatnaam,
            loc.Huisnummer,
            loc.Busnummer,
            loc.Postcode,
            loc.Gemeente,
            loc.Land,
            loc.Hoofdlocatie,
            loc.Locatietype);
}
