namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

/// <summary>
/// Het type van een vereniging
/// </summary>
[DataContract]
public class VerenigingsType
{
    /// <summary>
    /// De code van het type vereniging
    /// </summary>
    [DataMember(Name = "Code")]
    public string Code { get; set; } = null!;

    /// <summary>
    /// De beschrijving van het type vereniging
    /// </summary>
    [DataMember(Name = "Beschrijving")]
    public string Beschrijving { get; set; } = null!;
}