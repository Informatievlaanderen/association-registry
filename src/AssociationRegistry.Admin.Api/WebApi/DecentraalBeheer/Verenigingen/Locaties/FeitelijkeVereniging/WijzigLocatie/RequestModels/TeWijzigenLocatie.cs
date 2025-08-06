namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;

using AssociationRegistry.DecentraalBeheer.Locaties.WijzigLocatie;
using AssociationRegistry.Vereniging;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Adres = Common.Adres;
using AdresId = Common.AdresId;

/// <summary>Een locatie van een vereniging</summary>
[DataContract]
public class TeWijzigenLocatie
{
    /// <summary>
    ///     Het soort locatie dat beschreven wordt<br />
    ///     <br />
    ///     Mogelijke waarden:<br />
    ///     - Activiteiten<br />
    ///     - Correspondentie - Slechts één maal mogelijk<br />
    /// </summary>
    [DataMember]
    public string? Locatietype { get; set; }

    /// <summary>Duidt aan dat dit de primaire locatie is</summary>
    [DataMember]
    public bool? IsPrimair { get; set; }

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

    public static WijzigLocatieCommand.Locatie Map(TeWijzigenLocatie locatie, int locatieId)
        => new(
            locatieId,
            locatie.Locatietype is not null
                ? AssociationRegistry.Vereniging.Locatietype.Parse(locatie.Locatietype)
                : null,
            locatie.IsPrimair,
            locatie.Naam,
            locatie.Adres is not null
                ? AssociationRegistry.Vereniging.Adres.Create(
                    locatie.Adres.Straatnaam,
                    locatie.Adres.Huisnummer,
                    locatie.Adres.Busnummer,
                    locatie.Adres.Postcode,
                    locatie.Adres.Gemeente,
                    locatie.Adres.Land)
                : null,
            locatie.AdresId is not null
                ? AssociationRegistry.Vereniging.AdresId.Create(
                    locatie.AdresId.Broncode,
                    locatie.AdresId.Bronwaarde)
                : null);
}
