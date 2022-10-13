namespace AssociationRegistry.Public.Api.SearchVerenigingen;

using Nest;

public class VerenigingDocument
{
    [Keyword]
    public string VCode { get; set; }
    [Text]
    public string Naam { get; set; }
}
