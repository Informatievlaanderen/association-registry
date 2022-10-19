namespace AssociationRegistry.Public.Api.SearchVerenigingen;

using Nest;

public record VerenigingDocument(
    [property: Keyword] string VCode,
    [property: Text] string Naam,
    [property: Text] string KorteNaam,
    [property: Text] string Hoofdlocatie,
    [property: Text] string[] Locaties,
    [property: Keyword] string[] Hoofdactiviteiten,
    [property: Text] string Doelgroep,
    [property: Text] string[] Activiteiten
);
