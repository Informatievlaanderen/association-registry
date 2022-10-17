namespace AssociationRegistry.Public.Api.SearchVerenigingen;

using Nest;

public record VerenigingDocument(
    [property: Keyword] string VCode,
    [property: Text] string Naam);
