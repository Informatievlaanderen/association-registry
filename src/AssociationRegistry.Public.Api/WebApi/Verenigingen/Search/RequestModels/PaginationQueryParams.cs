namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.Search.RequestModels;

using AssociationRegistry.Public.Api.Constants;
using System.Runtime.Serialization;

[DataContract]
public class PaginationQueryParams
{
    /// <summary>
    /// Het aantal items dat overgeslagen zal worden
    /// </summary>
    [DataMember(Name = "Offset")]
    public int Offset { get; set; } = PagingConstants.DefaultOffset;

    /// <summary>
    /// Het aantal items dat (maximaal) zal worden opgehaald
    /// </summary>
    /// <remarks>
    /// De laatste pagina kan minder items bevatten
    /// </remarks>
    [DataMember(Name = "Limit")]
    public int Limit { get; set; } = PagingConstants.DefaultLimit;
}
