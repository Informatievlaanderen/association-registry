namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class GegevensInitiatorErkenning
{
    /// <summary>
    /// OVO-code van de initiator die deze erkenning heeft bevestigd.
    /// </summary>
    [DataMember(Name = "OvoCode")]
    public string OvoCode { get; set; }

    /// <summary>
    /// Naam van de initiator die deze erkenning heeft bevestigd.
    /// </summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; set; }
}
