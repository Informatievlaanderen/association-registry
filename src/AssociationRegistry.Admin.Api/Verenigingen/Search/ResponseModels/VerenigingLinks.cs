namespace AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class VerenigingLinks
{
    /// <summary>
    /// De link naar het beheer detail van de vereniging
    /// </summary>
    [DataMember(Name = "Detail")]
    public Uri Detail { get; init; } = null!;
}
