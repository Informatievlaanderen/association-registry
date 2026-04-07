namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class IpdcProduct
{
    [DataMember(Name = "Nummer")]
    public string Nummer { get; set; } = null!;

    [DataMember(Name = "Naam")]
    public string Naam { get; set; } = null!;
}
