namespace AssociationRegistry.Public.Api.SearchVerenigingen;

using Nest;

public record VerenigingDocument(
    [property: Keyword] string VCode,
    [property: Text] string Naam,
    [property: Text] string KorteNaam,
    [property: Nested] VerenigingDocument.Locatie Hoofdlocatie,
    [property: Nested] VerenigingDocument.Locatie[] Locaties,
    [property: Keyword] string[] Hoofdactiviteiten,
    [property: Text] string Doelgroep,
    [property: Text] string[] Activiteiten
)
{
    public record Locatie(
        [property: Text] string Postcode,
        [property: Text] string Gemeente);
}


