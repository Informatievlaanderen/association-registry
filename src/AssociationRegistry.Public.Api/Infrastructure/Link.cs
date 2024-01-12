namespace AssociationRegistry.Public.Api.Infrastructure;

using System.Runtime.Serialization;

public record Link(
    [property: DataMember(Name = "Href")] string Href,
    [property: DataMember(Name = "Type")] string Type,
    [property: DataMember(Name = "Rel")] string Rel
)
{
    public static Link VerenigingDetail(string verenigingId)
        => new($"/v1/verenigingen/static/{verenigingId}", Type: "GET", Rel: "Detail");
}
