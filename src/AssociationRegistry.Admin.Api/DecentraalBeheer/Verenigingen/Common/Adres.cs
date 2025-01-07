namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;

using System.Runtime.Serialization;

/// <summary>Een adres van een locatie</summary>
[DataContract]
public class Adres
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
