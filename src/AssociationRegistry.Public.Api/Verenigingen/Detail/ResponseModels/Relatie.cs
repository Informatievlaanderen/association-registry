namespace AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Relatie
{
    /// <summary>
    /// Het type relatie
    /// </summary>
    [DataMember(Name = "Type")]
    public string Type { get; set; } = null!;

    /// <summary>
    /// De waarde van de relatie
    /// </summary>
    [DataMember(Name = "Waarde")]
    public string Waarde { get; set; } = null!;
}
