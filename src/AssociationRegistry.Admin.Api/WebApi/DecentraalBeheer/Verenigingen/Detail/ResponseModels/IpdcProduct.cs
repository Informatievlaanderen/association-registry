namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class IpdcProduct
{
    /// <summary>
    /// IPDC-productnummer van de erkenning
    /// </summary>
    [DataMember(Name = "Nummer")]
    public string Nummer { get; set; } = null!;

    /// <summary>
    /// Naam van het IPDC-product
    /// </summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; set; } = null!;
}
