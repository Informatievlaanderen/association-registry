namespace AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class HoofdactiviteitVerenigingsloket
{
    /// <summary>
    /// De verkorte code van de hoofdactiviteit
    /// </summary>
    [DataMember]
    public string Code { get; set; } = null!;

    /// <summary>
    /// De volledige beschrijving van de hoofdactiviteit
    /// </summary>
    [DataMember]
    public string Beschrijving { get; set; } = null!;
}
