namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Vereniging.WijzigBasisgegevens;

[DataContract]
public class WijzigBasisgegevensRequest
{
    /// <summary>Instantie die de vereniging aanmaakt</summary>
    [DataMember]
    [Required]
    public string Initiator { get; init; } = null!;

    /// <summary>
    /// Nieuwe naam van de vereniging
    /// </summary>
    [DataMember]
    public string? Naam { get; set; }

    public WijzigBasisgegevensCommand ToWijzigBasisgegevensCommand(string vCode)
        => new(vCode, Naam);
}
