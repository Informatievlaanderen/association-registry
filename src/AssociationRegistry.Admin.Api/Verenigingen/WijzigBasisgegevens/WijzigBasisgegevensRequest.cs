namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Acties.WijzigBasisgegevens;
using Primitives;
using Vereniging;

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

    /// <summary>Nieuwe startdatum van de vereniging. Deze datum mag niet later zijn dan vandaag</summary>
    [DataMember]
    public NullOrEmpty<DateOnly> Startdatum { get; set; }

    public class Contactgegeven
    {
        [DataMember(Name = "type")] public string Type { get; set; } = null!;
        [DataMember(Name = "waarde")] public string Waarde { get; set; } = null!;
        [DataMember(Name = "omschrijving")] public string? Omschrijving { get; set; }

        [DataMember(Name = "isPrimair", EmitDefaultValue = false)]
        public bool IsPrimair { get; set; }
    }

    public WijzigBasisgegevensCommand ToCommand(string vCode)
        => new(
            VCode.Create(vCode),
            Naam is null ? null : VerenigingsNaam.Create(Naam),
            KorteNaam,
            KorteBeschrijving,
            Startdatum.IsNull ? null : AssociationRegistry.Vereniging.Startdatum.Create(Startdatum.Value)
        );
}
