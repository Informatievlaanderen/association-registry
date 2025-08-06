namespace AssociationRegistry.Admin.Api.WebApi.Administratie.Configuratie;

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
