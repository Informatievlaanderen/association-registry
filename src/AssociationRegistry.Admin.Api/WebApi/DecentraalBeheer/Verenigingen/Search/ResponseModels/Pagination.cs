namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Pagination
{
    /// <summary>
    /// Het totaal aantal verenigingen dat voldoet aan de zoekcriteria
    /// </summary>
    [DataMember(Name = "TotalCount")]
    public long TotalCount { get; init; }

    /// <summary>
    /// Het aantal overgeslagen resultaten
    /// </summary>
    [DataMember(Name = "Offset")]
    public int Offset { get; init; }

    /// <summary>
    /// Het maximum aantal teruggegeven resultaten
    /// </summary>
    [DataMember(Name = "Limit")]
    public int Limit { get; init; }
}
