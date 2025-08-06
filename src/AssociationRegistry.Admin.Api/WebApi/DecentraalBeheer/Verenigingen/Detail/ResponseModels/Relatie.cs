namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Relatie
{
    /// <summary>
    ///     Het type relatie
    /// </summary>
    [DataMember(Name = "Relatietype")]
    public string Relatietype { get; set; } = null!;

    /// <summary>
    /// De gerelateerde vereniging
    /// </summary>
    [DataMember(Name = "AndereVereniging")]
    public GerelateerdeVereniging AndereVereniging { get; set; } = null!;

    /// <summary>
    /// Het id van de relatie
    /// </summary>
    [DataMember(Name = "RelatieId")]
    public int RelatieId { get; set; }
}
