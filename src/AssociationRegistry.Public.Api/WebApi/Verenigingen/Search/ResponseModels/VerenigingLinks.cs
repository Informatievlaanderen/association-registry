namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.Search.ResponseModels;

using System;
using System.Runtime.Serialization;

[DataContract]
public class VerenigingLinks
{
    /// <summary>
    /// De link naar het publiek detail van de vereniging
    /// </summary>
    [DataMember(Name = "Detail")]
    public Uri Detail { get; init; } = null!;
}
