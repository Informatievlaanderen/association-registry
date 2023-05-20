namespace AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class VerenigingsType
{
    /// <summary>
    /// Code van het type vereniging
    /// </summary>
    [DataMember(Name = "Code")]
    public string Code { get; set; } = null!;

    /// <summary>
    /// Beschrijving van het type vereniging
    /// </summary>
    [DataMember(Name = "Beschrijving")]
    public string Beschrijving { get; set; } = null!;
}
