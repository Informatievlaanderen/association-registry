namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class GegevensInitiator
{
    /// <summary>
    /// OVO-code van de initiator die dit bankrekeningnummer heeft bevestigd.
    /// </summary>
    [DataMember(Name = "OvoCode")]
    public string OvoCode { get; set; }
}
