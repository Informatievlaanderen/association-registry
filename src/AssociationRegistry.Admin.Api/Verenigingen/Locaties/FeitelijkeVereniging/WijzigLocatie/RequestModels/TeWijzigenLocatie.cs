namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;

using System.Runtime.Serialization;
using Acties.WijzigLocatie;
using Common;

/// <summary>Een locatie van een vereniging</summary>
[DataContract]
public class TeWijzigenLocatie
{
    /// <summary>
    ///     Het soort locatie dat beschreven wordt<br />
    ///     <br />
    ///     Mogelijke waarden:<br />
    ///     - Activiteiten<br />
    ///     - Correspondentie - Slechtséén maal mogelijk<br />
    /// </summary>
    [DataMember]
    public string? Locatietype { get; set; } = null!;

    /// <summary>Duidt aan dat dit de primaire locatie is</summary>
    [DataMember]
    public bool? IsPrimair { get; set; }

    /// <summary>Een beschrijvende naam voor de locatie</summary>
    [DataMember]
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
                ? Vereniging.Locatietype.Parse(locatie.Locatietype)
                : null,
            locatie.IsPrimair,
            locatie.Naam,
            locatie.Adres is not null
                ? Vereniging.Adres.Create(
                    locatie.Adres.Straatnaam,
                    locatie.Adres.Huisnummer,
                    locatie.Adres.Busnummer,
                    locatie.Adres.Postcode,
                    locatie.Adres.Gemeente,
                    locatie.Adres.Land)
                : null,
            locatie.AdresId is not null
                ? Vereniging.AdresId.Create(
                    locatie.AdresId.Broncode,
                    locatie.AdresId.Bronwaarde)
                : null);
}
