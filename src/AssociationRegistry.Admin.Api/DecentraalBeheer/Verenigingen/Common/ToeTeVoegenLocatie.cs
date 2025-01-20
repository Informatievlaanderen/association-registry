namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using AssociationRegistry.Vereniging;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

/// <summary>Een locatie van een vereniging</summary>
[DataContract]
public class ToeTeVoegenLocatie
{
    /// <summary>
    ///     Het soort locatie dat beschreven wordt<br />
    ///     <br />
    ///     Mogelijke waarden:<br />
    ///     - Activiteiten<br />
    ///     - Correspondentie - Slechts één maal mogelijk<br />
    /// </summary>
    [DataMember]
    public string Locatietype { get; set; } = null!;

    /// <summary>Duidt aan dat dit de primaire locatie is</summary>
    [DataMember]
    public bool IsPrimair { get; set; }

    /// <summary>Een beschrijvende naam voor de locatie</summary>
    [DataMember]
    [MaxLength(Locatie.MaxLengthLocatienaam)]
    public string? Naam { get; set; }

    /// <summary>De unieke identificator van het adres in een andere bron</summary>
    [DataMember]
    public AdresId? AdresId { get; set; }

    /// <summary>Het adres van de locatie</summary>
    [DataMember]
    public Adres? Adres { get; set; }

    public static Locatie Map(ToeTeVoegenLocatie loc)
        => Locatie.Create(
            Locatienaam.Create(loc.Naam ?? string.Empty),
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
