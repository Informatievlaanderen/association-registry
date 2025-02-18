namespace AssociationRegistry.Admin.Api.Administratie.Configuratie;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.DuplicateVerenigingDetection;
using DubbelControle;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

[DataContract]
public class OverrideMinimumScoreDuplicateDetectionRequest
{
    /// <summary>Naam van de vereniging</summary>
    [DataMember]
    [Required]
    public double? Waarde { get; set; } = null!;
}
