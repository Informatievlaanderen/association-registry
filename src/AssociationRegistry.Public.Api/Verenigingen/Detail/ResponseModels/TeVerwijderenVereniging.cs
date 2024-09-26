namespace AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class TeVerwijderenVereniging
{
    [DataMember(Name = "vcode")]
    public string VCode { get; set; }
    [DataMember(Name = "teVerwijderen")]
    public bool TeVerwijderen { get; set; }
}
