namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Vereniging.WijzigBasisgegevens;

[DataContract]
public class WijzigBasisgegevensRequest
{
    /// <summary>Instantie die de vereniging wijzigt</summary>
    [DataMember]
    [Required]
    public string Initiator { get; init; } = null!;

    /// <summary>Nieuwe naam van de vereniging</summary>
    [DataMember]
    public string? Naam { get; set; }

    /// <summary>Nieuwe korte naam van de vereniging</summary>
    [DataMember]
    public string? KorteNaam { get; set; }

    /// <summary>Nieuwe korte beschrijving van de vereniging</summary>
    [DataMember]
    public string? KorteBeschrijving { get; set; }

    /// <summary>Nieuwe startdatum van de vereniging</summary>
    [DataMember]
    public DateOnly? Startdatum { get; set; }

    public WijzigBasisgegevensCommand ToWijzigBasisgegevensCommand(string vCode)
        => new(
            vCode,
            Naam: Naam,
            KorteNaam: KorteNaam,
            KorteBeschrijving: KorteBeschrijving,
            Startdatum: Startdatum);
}
