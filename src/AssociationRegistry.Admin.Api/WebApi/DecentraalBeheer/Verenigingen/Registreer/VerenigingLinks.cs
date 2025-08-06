namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer;

using System.Runtime.Serialization;

/// <summary>Weblinks i.v.m. deze vereniging</summary>
[DataContract]
public class VerenigingLinks
{
    public VerenigingLinks(Uri detail)
    {
        Detail = detail;
    }

    /// <summary>De link naar het beheer detail van de vereniging</summary>
    [DataMember(Name = "Detail")]
    public Uri Detail { get; init; }
}
